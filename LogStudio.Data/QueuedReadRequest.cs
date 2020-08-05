using System;
using System.Threading;

namespace LogStudio.Data
{
    public delegate void QueuedReadRequestEventHandler(long instanceID, string itemID, LogRowDataPoint[] data);

    internal class QueuedReadRequest : IAsyncResult, IDisposable
    {
        public int BatchSize { get; }
        public readonly string ItemID;
        public readonly int StartIndex;
        public readonly int Count;
        public event QueuedReadRequestEventHandler Callback;
        public readonly ManualResetEvent m_AsyncHandle = new ManualResetEvent(false);
        private bool m_Completed = false;

        internal LogRowDataPoint Data = null;

        internal QueuedReadRequest(string itemID, int startIndex, int count, int batchSize, QueuedReadRequestEventHandler callback)
        {
            BatchSize = batchSize;
            ItemID = itemID;
            StartIndex = startIndex;
            Count = count;
            Callback = callback;
            Data = null;
        }

        internal void SendResult(long instanceID, LogRowDataPoint[] points)
        {
            Callback?.BeginInvoke(instanceID, ItemID, points, null, null);

            m_Completed = true;
        }

        #region IAsyncResult Members

        object IAsyncResult.AsyncState
        {
            get { return this; }
        }

        System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle
        {
            get { return m_AsyncHandle; }
        }

        bool IAsyncResult.CompletedSynchronously
        {
            get { return false; }
        }

        bool IAsyncResult.IsCompleted
        {
            get { return m_Completed; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            m_AsyncHandle.Close();
        }

        #endregion
    }
}
