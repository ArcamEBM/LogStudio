using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LogStudio.Data
{
    internal class LogDataCache
    {
        private readonly Dictionary<long, WeakReference> m_DataPointCache = new Dictionary<long, WeakReference>();
        private readonly Dictionary<long, WeakReference> m_DataCache = new Dictionary<long, WeakReference>();

        private Stream m_Stream;
        private PagedStreamReader m_PagedReader;

        public LogDataCache(Stream stream)
        {
            m_Stream = stream;
            m_PagedReader = new PagedStreamReader(m_Stream, 4096);
        }

        public LogRowDataPoint GetDataPoint(LogRowIndex index)
        {
            return GetDataPoints(new[] { index }).First();
        }

        public IEnumerable<LogRowDataPoint> GetDataPoints(IEnumerable<LogRowIndex> indexes)
        {

            foreach (LogRowIndex logRowIndex in indexes)
            {
                if (!m_DataPointCache.TryGetValue(logRowIndex.RowStart, out var reference))
                {
                    LogRowDataPoint point = LogRowDataFactory.CreateDataPoint(m_PagedReader, logRowIndex);
                    reference = new WeakReference(point);
                    m_DataPointCache.Add(logRowIndex.RowStart, reference);
                }

                LogRowDataPoint dataPoint = reference.Target as LogRowDataPoint;

                //Reload data point
                if (dataPoint == null)
                {
                    dataPoint = LogRowDataFactory.CreateDataPoint(m_PagedReader, logRowIndex);
                    reference.Target = dataPoint;
                }

                yield return dataPoint;
            }
        }

        public LogRowData GetData(LogRowIndex index)
        {
            return GetData(new[] { index }).First();
        }

        public IEnumerable<LogRowData> GetData(IEnumerable<LogRowIndex> indexes)
        {
            foreach (LogRowIndex logRowIndex in indexes)
            {
                WeakReference reference;

                if (!m_DataCache.TryGetValue(logRowIndex.RowStart, out reference))
                {
                    reference = new WeakReference(null);
                    m_DataCache.Add(logRowIndex.RowStart, reference);
                }

                LogRowData data = reference.Target as LogRowData;

                //Reload data point
                if (data == null)
                {
                    data = LogRowDataFactory.CreateData(m_Stream, logRowIndex);
                    reference.Target = data;
                }

                yield return data;
            }
        }
    }
}
