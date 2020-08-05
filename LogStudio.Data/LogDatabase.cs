using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LogStudio.Data
{
    public partial class LogDatabase : Disposable, IItemDatabase
    {
        //public event EventHandler<ProgressEventArgs> OnParserProgress;
        private event EventHandler<ExceptionEventArgs> ParserReadError;
        public event EventHandler<MultipplePLGFilesArgs> MultipleLogFilesQuery;

        private readonly object m_ParserReadErrorSync = new object();

        private event EventHandler<LogIndexChangesEventArgs> IndexesChanged;
        private readonly object m_IndexesChangedSync = new object();

        private event EventHandler<LogIndexAddedEventArgs> IndexesAdded;

        private readonly object m_IndexesAddedSync = new object();

        public event EventHandler<LogIndexChangesEventArgs> OnIndexesChanged
        {
            add
            {
                lock (m_IndexesChangedSync)
                {
                    IndexesChanged += value;
                }
            }
            remove
            {
                lock (m_IndexesChangedSync)
                {
                    IndexesChanged -= value;
                }
            }
        }

        public event EventHandler<LogIndexAddedEventArgs> OnIndexesAdded
        {
            add
            {
                lock (m_IndexesAddedSync)
                {
                    IndexesAdded += value;
                }
            }
            remove
            {
                lock (m_IndexesAddedSync)
                {
                    IndexesAdded -= value;
                }
            }
        }

        public event EventHandler<ExceptionEventArgs> OnParserReadError
        {
            add
            {
                lock (m_ParserReadErrorSync)
                {
                    ParserReadError += value;
                }
            }
            remove
            {
                lock (m_ParserReadErrorSync)
                {
                    ParserReadError -= value;
                }
            }
        }

        public event EventHandler OnInitialReadDone;

        private static long _instanceId;

        private readonly LogIndexCollection m_LogIndexes = new LogIndexCollection();

        private readonly LogFileParser m_Parser;
        private Stream m_Stream;

        private QueuedRequestManager m_RequestManager;
        private LogDataCache m_LogDataCache;

        public LogDatabase(string filename, bool monitorFile, HashSet<string> allowedItems)
        {
            _instanceId++;

            Filename = filename;

            m_LogIndexes.OnIndexesChanged += OnIndexCountChangedCallback;
            m_LogIndexes.OnIndexesAdded += OnIndexesAddedCallback;

            m_Parser = new LogFileParser(filename, '|', monitorFile, m_LogIndexes, allowedItems);
            m_Parser.OnProgress += OnParserProgressCallback;
            m_Parser.OnParserReadError += OnParserReadErrorCallback;
            m_Parser.OnInitialReadDone += OnParserInitialReadDone;
            m_Parser.OnDecompressionDone += OnParserDecompressionDone;
            m_Parser.MultippleLogFilesQuery += OnMultipleLogFileQuery;
        }

        public int Count
        {
            get { return m_LogIndexes.Count; }
        }


        private void OnMultipleLogFileQuery(object sender, MultipplePLGFilesArgs e)
        {
            if (MultipleLogFilesQuery != null)
                MultipleLogFilesQuery(sender, e);
            else
                throw new InvalidDataException("Multiple process log files in a archive is not supported.");
        }

        private void OnParserDecompressionDone(object sender, EventArgs e)
        {
            m_Stream = new FileStream(m_Parser.Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 200);
            m_LogDataCache = new LogDataCache(m_Stream);
            m_RequestManager = new QueuedRequestManager(_instanceId, this.m_LogIndexes, m_Parser.Filename);
        }


        private void OnParserInitialReadDone(object sender, EventArgs e)
        {
            OnInitialReadDone?.Invoke(this, EventArgs.Empty);
        }

        public long InstanceID
        {
            get { return _instanceId; }
        }

        public ProgressStateEnum State { get; private set; }

        public int Progress { get; private set; }

        public int ElapsedTime { get; private set; }

        public float BytesPerSecond { get; private set; }


        private void OnParserReadErrorCallback(object sender, ExceptionEventArgs e)
        {
            Delegate[] delegates = new Delegate[0];
            lock (m_ParserReadErrorSync)
            {
                if (ParserReadError != null)
                    delegates = ParserReadError.GetInvocationList();
            }

            if (delegates.Length > 0)
            {
                foreach (Delegate d in delegates)
                {
                    d.DynamicInvoke(sender, e);
                }
            }
            else
                throw e.Exception;
        }

        public void Start()
        {
            m_Parser.BeginReadLogFile(null);
        }

        public void Read()
        {
            m_Parser.ReadLogFile();
        }

        private void OnIndexesAddedCallback(object sender, LogIndexAddedEventArgs e)
        {
            Delegate[] delegates = new Delegate[0];
            lock (m_IndexesAddedSync)
            {
                if (IndexesAdded != null)
                    delegates = IndexesAdded.GetInvocationList();
            }

            foreach (Delegate d in delegates)
            {
                d.DynamicInvoke(sender, e);
            }
        }

        private void OnIndexCountChangedCallback(object sender, LogIndexChangesEventArgs e)
        {
            Delegate[] delegates = new Delegate[0];
            lock (m_IndexesChangedSync)
            {
                if (IndexesChanged != null)
                    delegates = IndexesChanged.GetInvocationList();
            }

            foreach (Delegate d in delegates)
            {
                d.DynamicInvoke(sender, e);
            }
        }

        private void OnParserProgressCallback(object sender, ProgressEventArgs e)
        {
            //if (OnParserProgress != null)
            //    OnParserProgress(sender, e);

            State = e.State;
            Progress = e.Progress;
            ElapsedTime = (int)e.ElapsedTime.TotalSeconds;
            BytesPerSecond = e.BytesPerSecond;
        }

        public string Filename { get; }

        public IEnumerable<string> GetItemsIDs()
        {
            return m_LogIndexes.GetItemsIDs();
        }

        /// <summary>
        /// Get the amount of items in 
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public int GetItemRowCount(string itemId)
        {
            return m_LogIndexes.GetItemRowCount(itemId);
        }

        /// <summary>
        /// Look up a specific row from an item
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public LogRowData GetItemRow(string itemId, int index)
        {
            return m_LogDataCache.GetData(m_LogIndexes.GetItemRow(itemId, index));
        }

        /// <summary>
        /// Look up specific row from an unique Id
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="uniqueId"></param>
        /// <returns></returns>
        public LogRowData GetItemRow(string itemId, long uniqueId)
        {
            return m_LogDataCache.GetData(m_LogIndexes.FindIndices(itemId, index => index.RowStart == uniqueId))
                .First();
        }

        /// <summary>
        /// Get all rows in an item
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        private LogRowData[] GetItemRows(string itemId)
        {
            LogRowIndex[] indexes = m_LogIndexes.GetItemRows(itemId);

            return m_LogDataCache.GetData(indexes).ToArray();
        }

        /// <summary>
        /// Get all rows in an item from the given interval
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="fromIndex"></param>
        /// <param name="length" />
        /// <returns></returns>
        public LogRowData[] GetItemRows(string itemId, int fromIndex, int length)
        {
            LogRowIndex[] indexes = m_LogIndexes.GetItemRows(itemId, fromIndex, length);

            return m_LogDataCache.GetData(indexes).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="batchSize"></param>
        /// <param name="fromIndex"></param>
        /// <param name="length"></param>
        /// <param name="callback"></param>
        public void BeginGetItemRowsDP(string itemId, int batchSize, int fromIndex, int length,
            QueuedReadRequestEventHandler callback)
        {
            var request = new QueuedReadRequest(itemId, fromIndex, length, batchSize, callback);
            m_RequestManager.QueueRequest(request);
        }

        /// <summary>
        /// Returns the logrows unique ids. This can be used to sort the rows from several items.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public long[] GetLogRowUniqueID(string itemId)
        {
            LogRowIndex[] indexes = m_LogIndexes.GetItemRows(itemId);

            long[] rowIDs = new long[indexes.Length];

            for (int index = 0; index < indexes.Length; index++)
            {
                rowIDs[index] = indexes[index].RowStart;
            }

            return rowIDs;
        }

        /// <summary>
        /// Returns the logrows unique ids. This can be used to sort the rows from several items.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="fromIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public long[] GetLogRowUniqueID(string itemId, int fromIndex, int length)
        {
            LogRowIndex[] indexes = m_LogIndexes.GetItemRows(itemId, fromIndex, length);

            long[] rowIDs = new long[indexes.Length];

            for (int index = 0; index < indexes.Length; index++)
            {
                rowIDs[index] = indexes[index].RowStart;
            }

            return rowIDs;
        }

        public int GetRowIndex(long uniqueId)
        {
            LogRowIndex[] indexes = m_LogIndexes.GetCompleteIndexList();

            for (int i = 0; i < indexes.Length; i++)
            {
                if (indexes[i].RowStart == uniqueId)
                    return i;
            }

            return -1;
        }

        public IEnumerable<LogRowData> GetRange(int startIndex, int count)
        {
            LogRowIndex[] indexes = m_LogIndexes.GetItemRows(startIndex, count);

            return indexes.Select(logRowIndex => m_LogDataCache.GetData(logRowIndex));
        }

        public void ForEach(string itemId, Action<LogRowDataPoint> row)
        {
            LogRowIndex[] indexes = m_LogIndexes.GetItemRows(itemId);

            foreach (LogRowIndex index in indexes)
            {
                row(m_LogDataCache.GetDataPoint(index));
            }
        }

        public IEnumerable<LogRowData> GetAll(string itemId)
        {
            LogRowIndex[] indexes = m_LogIndexes.GetItemRows(itemId);

            return indexes.Select(index => m_LogDataCache.GetData(index));
        }

        public IEnumerable<LogRowDataPoint> GetAllDP(string itemId)
        {
            LogRowIndex[] indexes = m_LogIndexes.GetItemRows(itemId);

            return indexes.Select(index => m_LogDataCache.GetDataPoint(index));
        }

        public IEnumerable<LogRowData> GetCycleState(long cycleIndex)
        {
            //TODO: Improve performance by using a quick fint algorithm instead of FindAll
            return GetItemsIDs().Select(itemsId => FindAll(itemsId, data => data.CycleIndex <= cycleIndex))
                .Select(rows => rows.Last());
        }

        public string[] FindReplacedItem(string itemId)
        {
            // Check if item is in ItemReplacements.data
            return m_Parser.GetOriginalItemNames(itemId);
        }

        public IEnumerable<LogRowData> FindAll(string itemId, Predicate<LogRowData> row)
        {
            // Check if item is in ItemReplacements.data
            m_Parser.ReplaceItemname(ref itemId);

            LogRowIndex[] indexes = m_LogIndexes.GetItemRows(itemId);

            return indexes
                .Select(index => m_LogDataCache.GetData(index))
                .Where(data => row(data));
        }

        public IEnumerable<LogRowDataPoint> FindAll(string itemId, Predicate<LogRowDataPoint> row)
        {
            LogRowIndex[] indexes = m_LogIndexes.GetItemRows(itemId);

            return indexes
                .Select(index => m_LogDataCache.GetDataPoint(index))
                .Where(data => row(data));
        }


        public ItemProperties GetItemProperties(string fullname)
        {
            return m_LogIndexes.GetItemProperties(fullname);
        }


        #region IDisposable Members

        protected override void OnDispose()
        {
            m_LogIndexes.OnIndexesChanged -= OnIndexCountChangedCallback;
            m_LogIndexes.OnIndexesAdded -= OnIndexesAddedCallback;

            m_Parser.OnProgress -= OnParserProgressCallback;
            m_Parser.OnParserReadError -= OnParserReadErrorCallback;
            m_Parser.OnInitialReadDone -= OnParserInitialReadDone;
            m_Parser.OnDecompressionDone -= OnParserDecompressionDone;
            m_Parser.MultippleLogFilesQuery -= OnMultipleLogFileQuery;


            if (m_RequestManager != null)
            {
                m_RequestManager.Dispose();
                m_RequestManager = null;
            }

            m_Stream?.Dispose();

            m_Parser?.Dispose();
        }

        #endregion


        public bool Exists(string itemId)
        {
            return m_LogIndexes.Exists(itemId);
        }

        #region IItemDatabase Members

        public void GetItemRange(string itemId, out DateTime from, out DateTime to)
        {
            int count = m_LogIndexes.GetItemRowCount(itemId);

            LogRowDataPoint f = m_LogDataCache.GetDataPoint(m_LogIndexes.GetItemRow(itemId, 0));

            LogRowDataPoint t = f;
            if (count > 1)
            {
                t = m_LogDataCache.GetDataPoint(m_LogIndexes.GetItemRow(itemId, count - 1));
            }

            from = f.TimeStamp;
            to = t.TimeStamp;
        }

        public LogRowData GetLastItem(string itemId)
        {
            return m_LogDataCache.GetData(m_LogIndexes.GetLastIndex(itemId));
        }

        public LogRowData GetLastItem(string itemId, Predicate<LogRowData> predicate)
        {
            LogRowIndex[] indexes = m_LogIndexes.GetItemRows(itemId);

            return indexes
                .Reverse()
                .Select(index => m_LogDataCache.GetData(index))
                .FirstOrDefault(point => predicate(point));
        }

        public LogRowDataPoint GetLastItemDP(string itemId)
        {
            return m_LogDataCache.GetDataPoint(m_LogIndexes.GetLastIndex(itemId));
        }

        public LogRowDataPoint GetLastItemDP(string itemId, Predicate<LogRowDataPoint> predicate)
        {
            LogRowIndex[] indexes = m_LogIndexes.GetItemRows(itemId);

            return indexes
                .Reverse()
                .Select(index => m_LogDataCache.GetDataPoint(index))
                .FirstOrDefault(point => predicate(point));
        }

        public LogRowDataPoint GetFirstItemDP(string itemId, Predicate<LogRowDataPoint> predicate)
        {
            LogRowIndex[] indexes = m_LogIndexes.GetItemRows(itemId);

            return indexes
                .Select(index => m_LogDataCache.GetDataPoint(index))
                .FirstOrDefault(point => predicate(point));
        }

        public LogRowData GetFirstItem(string itemId, Predicate<LogRowData> predicate)
        {
            LogRowIndex[] indexes = m_LogIndexes.GetItemRows(itemId);

            return indexes
                .Select(index => m_LogDataCache.GetData(index))
                .FirstOrDefault(data => predicate(data));
        }

        public IEnumerable<NamedLogRow> GetAllDP(IEnumerable<KeyValuePair<string, LogRowType>> namedValuePairs)
        {
            var items = namedValuePairs.SelectMany(namedValuePair => m_LogIndexes.GetItemRows(namedValuePair.Key)
                    .Select(index => new
                    {
                        Fullname = namedValuePair.Key,
                        Index = index,
                        Dp = (namedValuePair.Value == LogRowType.Point)
                            ? (LogRow)m_LogDataCache.GetDataPoint(index)
                            : (LogRow)m_LogDataCache.GetData(index)
                    }))
                .OrderBy(enumerable => enumerable.Index.RowStart)
                .Select(item => new NamedLogRow(item.Fullname, item.Dp));

            foreach (NamedLogRow namedDataPoint in items)
            {
                yield return namedDataPoint;
            }
        }

        #endregion
    }
}