using System;
using System.Collections.Generic;

namespace LogStudio.Data
{
    public interface IItemDatabase : IDisposable
    {
        long InstanceID { get; }

        string Filename { get; }
        int Count { get; }


        bool Exists(string itemId);
        void GetItemRange(string itemId, out DateTime from, out DateTime to);

        LogRowData GetItemRow(string itemId, int index);
        LogRowData GetItemRow(string itemId, long uniqueId);

        int GetItemRowCount(string itemId);
        LogRowData[] GetItemRows(string itemId, int fromIndex, int length);

        /// <summary>
        /// Get items async
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="batchSize">Max items that will be sent back to callback in each batch</param>
        /// <param name="fromIndex"></param>
        /// <param name="length"></param>
        /// <param name="callback"></param>
        void BeginGetItemRowsDP(string itemId, int batchSize, int fromIndex, int length,
            QueuedReadRequestEventHandler callback);
        //LogRowDataPoint[] EndGetItemRowsDP(IAsyncResult ar);

        LogRowData GetLastItem(string itemId);
        LogRowData GetLastItem(string itemId, Predicate<LogRowData> predicate);
        LogRowDataPoint GetLastItemDP(string itemId);
        LogRowDataPoint GetLastItemDP(string itemId, Predicate<LogRowDataPoint> predicate);

        LogRowData GetFirstItem(string itemId, Predicate<LogRowData> predicate);
        LogRowDataPoint GetFirstItemDP(string itemId, Predicate<LogRowDataPoint> predicate);


        IEnumerable<string> GetItemsIDs();
        long[] GetLogRowUniqueID(string itemId);
        long[] GetLogRowUniqueID(string itemId, int fromIndex, int length);
        int GetRowIndex(long uniqueId);

        IEnumerable<LogRowData> FindAll(string itemId, Predicate<LogRowData> row);
        IEnumerable<LogRowData> GetAll(string itemId);
        IEnumerable<LogRowData> GetRange(int startIndex, int count);

        void ForEach(string itemId, Action<LogRowDataPoint> row);
        IEnumerable<LogRowDataPoint> GetAllDP(string itemId);


        void ExportToCSV(string filename, string itemId, char separator);


        void ExportStateOnItemChangeToCSV(string filename, string keyItem);
        ItemProperties GetItemProperties(string fullname);

        event EventHandler<LogIndexAddedEventArgs> OnIndexesAdded;
        event EventHandler<LogIndexChangesEventArgs> OnIndexesChanged;
        event EventHandler OnInitialReadDone;
    }

    public enum LogRowType
    {
        /// <summary>
        /// Log row data point as double with timestamp
        /// </summary>
        Point,

        /// <summary>
        /// Log row data as string with timestamp
        /// </summary>
        Raw
    }

    public class NamedLogRow
    {
        public string Fullname { get; }
        private readonly LogRow m_LogRow;

        public NamedLogRow(string fullname, LogRow logRow)
        {
            Fullname = fullname;
            m_LogRow = logRow;
        }

        public LogRowDataPoint GetAsDataPoint => m_LogRow as LogRowDataPoint;

        public LogRowData GetAsDataRow => m_LogRow as LogRowData;
    }
}