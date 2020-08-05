using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;


namespace LogStudio.Data
{
    internal class LogFileParser : Disposable
    {

        private const int NetworkFileReadBufferSize = 512 * 1024;
        private const int LocalFileReadBufferSize = 128 * 1024;
        private const int ParserBufferSize = 128 * 1024;

        public event EventHandler<ProgressEventArgs> OnProgress;
        public event EventHandler OnDecompressionDone;
        public event EventHandler OnInitialReadDone;
        public event EventHandler<ExceptionEventArgs> OnParserReadError;
        public event EventHandler<MultipplePLGFilesArgs> MultippleLogFilesQuery;


        public string Filename { get; private set; }
        private char m_Separator = '|';

        private long m_StartPos = 0;
        private TempFileManager m_TempFileManager = new TempFileManager();
        private LogIndexCollection m_Indexes;

        private long m_Position = 0;
        private long m_Length = 0;
        private DateTime m_ParseStarted;

        private Thread m_ReaderThread = null;

        private bool m_Abort = false;

        private char[] m_RowBuffer = new char[ParserBufferSize];
        private int m_RowBufferCount = 0;

        private HashSet<string> m_FilteredItems;

        private bool m_MonitorFile = false;

        private int m_LastProgress = 0;

        private Dictionary<string, string> m_ItemReplacements = new Dictionary<string, string>();

        private ILog m_Log = LogManager.GetLogger(typeof(LogFileParser));

        internal LogFileParser()
        {

        }

        internal LogFileParser(string filename, char separator, bool monitorFile, LogIndexCollection indexes, HashSet<string> filteredItems)
        {
            Filename = filename;
            m_Separator = separator;
            m_MonitorFile = monitorFile;

            m_Indexes = indexes;

            m_FilteredItems = filteredItems;

            ReadItemReplacements();
        }

        private void ReadItemReplacements()
        {
            string replacementsFile = Path.GetFullPath("ItemReplacements.data");
            if (File.Exists(replacementsFile))
            {
                using (StreamReader reader = new StreamReader(File.Open(replacementsFile, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        line.Trim(' ');

                        string[] parts = line.Split('=');

                        if (parts.Length == 2)
                        {
                            m_ItemReplacements.Add(parts[0], parts[1]);
                        }
                    }
                }
            }
        }

        internal IAsyncResult BeginReadLogFile(AsyncCallback callback)
        {
            m_Abort = false;
            m_ReaderThread = new Thread(OnReaderThreadExecute)
            {
                Name = "Reader Thread",
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal,
            };

            AsyncResult result = new AsyncResult(new AsyncState(callback));

            m_ReaderThread.Start(result);

            return result;
        }

        internal void EndReadLogFile(IAsyncResult ar)
        {
            AsyncResult result = ar as AsyncResult;
            if (result != null)
            {
                if (result.State.Exception != null)
                    throw result.State.Exception;
            }
        }

        public void ReadLogFile()
        {
            m_MonitorFile = false;
            ParseLogFile();
        }

        private void OnReaderThreadExecute(object state)
        {
            try
            {
                ParseLogFile();

                AsyncResult result = state as AsyncResult;

                if (result != null && result.State.Callback != null)
                    result.State.Callback(result);
            }
            catch (Exception ex)
            {
                m_Log.Error(ex.Message, ex);
                throw;
            }
        }

        private void ParseLogFile()
        {
            int bufferSize = LocalFileReadBufferSize;

            if (Filename.StartsWith(@"\\"))
            {
                //UNC is presumed to be networked
                bufferSize = NetworkFileReadBufferSize;
            }
            else
            {
                DriveInfo driveInfo = new DriveInfo(Filename);
                if (driveInfo.DriveType == DriveType.Network)
                    bufferSize = NetworkFileReadBufferSize;
            }

            try
            {
                if (Filename.ToLower().EndsWith(".zip"))
                {
                    Filename = GetUnZippedFilename(Filename);
                }

                if (OnDecompressionDone != null)
                    OnDecompressionDone(this, EventArgs.Empty);

                if (OnProgress != null)
                    OnProgress(this, new ProgressEventArgs(ProgressStateEnum.Starting));

                FileInfo monitoredFile = new FileInfo(Filename);

                long lastSize = monitoredFile.Length;

                ReadFromFile(bufferSize);

                if (OnInitialReadDone != null)
                    OnInitialReadDone(this, EventArgs.Empty);

                while (m_MonitorFile && !m_Abort)
                {
                    MonitorProgress();
                    Thread.CurrentThread.Join(1000);
                    monitoredFile.Refresh();
                    if (monitoredFile.Length > lastSize)
                    {
                        ReadFromFile(bufferSize);
                        ClearProgress();
                    }
                }
            }
            catch (Exception ex)
            {
                if (OnParserReadError != null)
                    OnParserReadError(this, new ExceptionEventArgs(ex));
            }
            finally
            {
                if (OnProgress != null)
                    OnProgress(this, new ProgressEventArgs(ProgressStateEnum.Done, DateTime.Now.Subtract(m_ParseStarted)));
            }
        }

        private void ReadFromFile(int bufferSize)
        {
            byte[] readBuffer = new byte[bufferSize];
            byte[] buffer = new byte[bufferSize];
            RowIndexCollection indexCollection = new RowIndexCollection();

            try
            {
                using (Stream stream = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1024 * 1024, FileOptions.Asynchronous | FileOptions.SequentialScan))
                {
                    //Test log file for invalid null
                    if (m_Position == 0)
                    {
                        stream.Seek(-1, SeekOrigin.End);
                        int value = stream.ReadByte();
                        if (value == 0)
                            throw new IOException("Invalid null in log file!");
                    }

                    stream.Position = m_Position;

                    CleanFromUTF8FormatEncoding(stream);

                    int version = CheckVersion(stream);

                    m_ParseStarted = DateTime.UtcNow;
                    m_Length = stream.Length;

                    IAsyncResult ar = stream.BeginRead(readBuffer, 0, readBuffer.Length, null, null);
                    while (!m_Abort)
                    {
                        int read = stream.EndRead(ar);
                        if (read == 0)
                            break;

                        Buffer.BlockCopy(readBuffer, 0, buffer, 0, read);

                        ar = stream.BeginRead(readBuffer, 0, readBuffer.Length, null, null);

                        ParseBuffer(buffer, read, indexCollection);

                        m_Position += read;

                        SendProgress(ProgressStateEnum.InProgress, m_Position, m_Length, m_ParseStarted);
                    }
                }
            }
            finally
            {
                if (indexCollection.Count > 0)
                {
                    SendItems(indexCollection);
                }
            }
        }

        private int CheckVersion(Stream stream)
        {
            long position = stream.Position;

            StreamReader reader = new StreamReader(stream);

            string line = reader.ReadLine();
            if (line != null && line.StartsWith("#-Version: "))
            {
                line = line.Remove(0, "#-Version: ".Length);
                reader.DiscardBufferedData();
                return int.Parse(line);
            }

            stream.Position = position;
            return 1;
        }

        private void CleanFromUTF8FormatEncoding(Stream stream)
        {
            if (stream.Position == 0)
            {
                byte[] buffer = new byte[3];
                int read = stream.Read(buffer, 0, buffer.Length);

                if (!(((read >= 3) && (buffer[0] == 0xef)) && ((buffer[1] == 0xbb) && (buffer[2] == 0xbf))))
                {
                    stream.Position = 0;
                }
            }
        }

        internal void ParseBuffer(byte[] buffer, int bufferLength, RowIndexCollection indexCollection)
        {
            const byte nl = (byte)'\n';
            //Loop through all characters in the buffer
            int bufferIndex = 0;
            while (bufferIndex < bufferLength && m_RowBufferCount < m_RowBuffer.Length)
            {
                m_RowBuffer[m_RowBufferCount++] = (char)buffer[bufferIndex];

                if (buffer[bufferIndex++] == nl)
                {
                    ParseRowBuffer(m_RowBuffer, indexCollection);

                    m_StartPos += m_RowBufferCount;

                    m_RowBufferCount = 0;
                    //SendItems(items);
                }
            }
        }

        internal void ParseRowBuffer(char[] rowBuffer, RowIndexCollection indexCollection)
        {
            if (rowBuffer[0] != '#')
            {
                string itemID;
                LogRowIndex index;

                ParseRow(rowBuffer, m_RowBufferCount, m_StartPos, m_Separator, out index, out itemID);

                ReplaceItemname(ref itemID);

                AddLogRowIndex(itemID, indexCollection, index);
            }
            else
                ParseComment(rowBuffer, m_RowBufferCount, m_StartPos - m_RowBufferCount, (ushort)m_RowBufferCount);
        }

        private void AddLogRowIndex(string itemID, RowIndexCollection indexCollection, LogRowIndex index)
        {
            if (m_FilteredItems == null)
            {
                indexCollection.Add(itemID, index);
            }
            else if (m_FilteredItems.Contains(itemID))
            {
                indexCollection.Add(itemID, index);
            }
        }

        public void ReplaceItemname(ref string itemId)
        {
            if (m_ItemReplacements != null && m_ItemReplacements.ContainsKey(itemId))
            {
                itemId = m_ItemReplacements[itemId];
            }
        }

        public string[] GetOriginalItemNames(string itemId)
        {
            if (m_ItemReplacements != null && m_ItemReplacements.ContainsValue(itemId))
            {
                return m_ItemReplacements.Where(item => item.Value == itemId).Select(i => i.Key).ToArray();
            }

            return Array.Empty<string>();
        }

        private void ParseComment(char[] rowbuffer, int rowLength, long startPos, ushort length)
        {
            string row = new string(rowbuffer, 0, rowLength);

            if (row.StartsWith("#-IOItem: "))
            {
                row = row.Remove(0, "#-IOItem: ".Length);

                ItemProperties property = new ItemProperties(row);

                m_Indexes.AddProperties(property);
            }
        }

        internal static void ParseRow(char[] rowBuffer, int rowLength, long startPos, char separator, out LogRowIndex index, out string itemID)
        {
            ushort[] lengths = new ushort[3];

            int rowIndex = LogRowIndex.TimeStampLength;

            for (var i = 0; i < lengths.Length; i++)
            {
                int lastIndex = rowIndex;
                rowIndex = rowBuffer.IndexOf(separator, rowIndex + 1);
                lengths[i] = (ushort)(rowIndex - 1 - lastIndex);
            }

            itemID = new string(rowBuffer, LogRowIndex.TimeStampLength + 1, lengths[0]);

            index = new LogRowIndex(startPos, (ushort)rowLength, lengths[0], lengths[1], (byte)lengths[2]);
        }

        public void SendItems(RowIndexCollection indexCollection)
        {
            m_Indexes.AddIndexs(indexCollection);
        }

        private void ClearProgress()
        {
            if (OnProgress != null)
                OnProgress(this, new ProgressEventArgs(ProgressStateEnum.Done, 0, TimeSpan.Zero, 0f));
        }

        private void MonitorProgress()
        {
            if (OnProgress != null)
                OnProgress(this, new ProgressEventArgs(ProgressStateEnum.Monitoring, 0, TimeSpan.Zero, 0f));
        }

        private void SendProgress(ProgressStateEnum state, long current, long max, DateTime start)
        {
            int progress = (int)(((double)current / (double)max) * 100.0);
            if (m_LastProgress != progress)
            {
                TimeSpan elapsedtime = DateTime.UtcNow.Subtract(start);

                double bytesPerSec = (double)current / elapsedtime.TotalSeconds;

                if (OnProgress != null)
                    OnProgress.BeginInvoke(this, new ProgressEventArgs(state, progress, elapsedtime, (float)bytesPerSec), null, null);

                m_LastProgress = progress;
            }
        }

        private string GetUnZippedFilename(string filename)
        {
            string tempFile = m_TempFileManager.GetTempFile();
            using (ZipArchive archive = ZipFile.OpenRead(filename))
            {
                if (archive.Entries == null)
                {
                    throw new IOException("Could not find any plg file.");
                }

                var plgCandidates = archive.Entries.Where(x => x.Name.EndsWith(".plg"));

                if (plgCandidates.Count() > 1)
                {
                    if (MultippleLogFilesQuery == null)
                    {
                        throw new InvalidDataException("Multiple process log files in a archive is not supported.");
                    }

                    MultipplePLGFilesArgs args = new MultipplePLGFilesArgs(plgCandidates.Select(x => x.FullName).ToArray());
                    MultippleLogFilesQuery(this, args);

                    if (args.Cancel)
                        throw new IOException("Operation aborted by user!");

                    if (string.IsNullOrEmpty(args.SelectedFilename))
                        throw new ArgumentException("No filename selected, operation aborted!");

                    plgCandidates = plgCandidates.Where(x => x.FullName.Equals(args.SelectedFilename));
                }

                if (plgCandidates.Count() == 0)
                {
                    throw new IOException("Could not find any plg file.");
                }

                plgCandidates.First().ExtractToFile(tempFile, true);
            }

            return tempFile;
        }


        protected override void OnDispose()
        {
            m_Abort = true;

            if (m_ReaderThread != null && m_ReaderThread.IsAlive)
                m_ReaderThread.Join();

            m_Indexes = null;
        }
    }

    internal class AsyncState
    {
        public AsyncState(AsyncCallback callback)
        {
            Callback = callback;
        }

        public AsyncCallback Callback { get; private set; }

        public Exception Exception { get; set; }
    }

    internal class AsyncResult : Disposable, IAsyncResult
    {
        private readonly EventWaitHandle m_Handle;

        public AsyncResult(AsyncState state)
        {
            State = state;
            m_Handle = new EventWaitHandle(false, EventResetMode.ManualReset);
        }

        public AsyncState State { get; private set; }

        #region IAsyncResult Members

        public object AsyncState
        {
            get { return State; }
        }

        public WaitHandle AsyncWaitHandle
        {
            get { return m_Handle; }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        public bool IsCompleted
        {
            get { return m_Handle.WaitOne(0); }
        }

        #endregion

        protected override void OnUnManagedDispose()
        {
            if (!m_Handle.SafeWaitHandle.IsClosed)
                m_Handle.Close();
        }
    }
}
