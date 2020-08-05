using System;
using System.Collections.Generic;
using System.IO;

namespace LogStudio.Data
{
    public class PagedStreamReader
    {
        private readonly Stream m_Stream;
        private readonly int m_PageSize;
        private long m_LastStreamSize;

        private readonly List<Page> m_Pages = new List<Page>();


        public PagedStreamReader(Stream stream, int pageSize)
        {
            m_Stream = stream;

            m_LastStreamSize = m_Stream.Length;

            m_PageSize = pageSize;

            int pages = (int)Math.Ceiling(m_LastStreamSize / (double)m_PageSize);

            for (var i = 0; i < pages; i++)
            {
                m_Pages.Add(new Page(i, m_PageSize, stream));
            }
        }

        /// <summary>
        /// Find a page by index
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        internal Page FetchPage(int pageIndex)
        {
            return m_Pages[pageIndex];
        }

        /// <summary>
        /// Adopt the pages to reflect changes of stream size
        /// </summary>
        public void ReEvaluatePageSize()
        {
            long streamSize = m_Stream.Length;

            if (m_LastStreamSize == streamSize)
                return;

            int pages = (int)Math.Ceiling(streamSize / (double)m_PageSize);

            if (m_Pages.Count > 0) //Make sure that the last page cache is released
                m_Pages[m_Pages.Count - 1].ReleaseCachedData();

            for (int i = m_Pages.Count; i < pages; i++)
            {
                m_Pages.Add(new Page(i, m_PageSize, m_Stream));
            }

            m_LastStreamSize = streamSize;
        }

        /// <summary>
        /// Read data into buffer from a specified startPosition in the stream
        /// </summary>
        /// <param name="buffer">Target buffer of data</param>
        /// <param name="startPos">Start read position in stream</param>
        /// <param name="length">Maximum length to read</param>
        /// <returns>Amount of bytes actually read</returns>
        public int Read(byte[] buffer, long startPos, int length)
        {
            int remain = length;
            int bufferPos = 0;

            ReEvaluatePageSize();

            while (remain > 0)
            {
                var read = ReadPage(buffer, bufferPos, startPos, remain);
                if (read <= 0)
                    return length - remain;
                remain -= read;
                bufferPos += read;
                startPos += read;
            }

            return length - remain;
        }

        /// <summary>
        /// Read data from a page, if less data is available in the page the return value will be the amount of read data.
        /// </summary>
        /// <param name="buffer">Target buffer</param>
        /// <param name="bufferIndex">Start index in the target buffer</param>
        /// <param name="startPos">Start read position in the stream</param>
        /// <param name="length">Maximum length to read</param>
        /// <returns>Amount of bytes actually read</returns>
        internal int ReadPage(byte[] buffer, int bufferIndex, long startPos, int length)
        {
            int pageIndex = (int)(startPos / m_PageSize);
            int pagePos = (int)(startPos % m_PageSize);

            var page = FetchPage(pageIndex);
            var data = page.Data;

            length = Math.Min(length, data.Length - pagePos);

            if (length <= 0)
                return -1;

            Buffer.BlockCopy(data, pagePos, buffer, bufferIndex, length);

            return length;
        }
    }

    public class Page
    {
        public long PageIndex { get; }

        public Page(int pageIndex, int pageSize, Stream mStream)
        {
            PageIndex = pageIndex;
            m_PageSize = pageSize;
            m_Stream = mStream;
        }

        private readonly WeakReference<byte[]> m_Data = new WeakReference<byte[]>(null);
        private readonly int m_PageSize;
        private readonly Stream m_Stream;

        public byte[] Data
        {
            get
            {
                if (!m_Data.TryGetTarget(out byte[] data))
                {
                    data = new byte[m_PageSize];
                    long pos = m_PageSize * PageIndex;

                    if (m_Stream.Position != pos)
                        m_Stream.Position = pos;

                    var read = m_Stream.Read(data, 0, m_PageSize);

                    if (read < m_PageSize)
                        Array.Resize(ref data, read);

                    m_Data.SetTarget(data);
                }

                return data;
            }
        }

        public void ReleaseCachedData()
        {
            m_Data.SetTarget(null);
        }
    }
}
