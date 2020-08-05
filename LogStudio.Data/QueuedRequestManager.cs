using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace LogStudio.Data
{
    internal class QueuedRequestManager : IDisposable
    {
        private Thread m_QueuedReadThread;
        private object m_SyncObject = new object();

        //private ManualResetEvent m_QueuedReadThreadQuitEvent;
        //private ManualResetEvent m_RequestAdded = new ManualResetEvent(false);
        private ManualResetEvent m_Event = new ManualResetEvent(false);
        private ManualResetEvent m_Quit = new ManualResetEvent(false);

        private LogIndexCollection m_Indexes;
        private string m_Filename;

        private List<QueuedReadRequest> m_Queue = new List<QueuedReadRequest>();
        private long m_DatabaseInstanceID = 0;
        private ILog m_Log = LogManager.GetLogger(typeof(QueuedRequestManager));

        internal QueuedRequestManager(long databaseInstanceID, LogIndexCollection indexes, string filename)
        {
            m_DatabaseInstanceID = databaseInstanceID;
            m_Indexes = indexes;
            m_Filename = filename;


            //m_QueuedReadThreadQuitEvent = new ManualResetEvent(false);
            Trace.TraceInformation("QueuedReadThread created: {0}", m_DatabaseInstanceID);
            m_QueuedReadThread = new Thread(new ThreadStart(OnQueuedReadThreadExecute))
            {
                Name = "Queued Request Manager"
            };
            m_QueuedReadThread.Start();
        }

        public void QueueRequest(QueuedReadRequest request)
        {
            lock (m_SyncObject)
            {
                m_Queue.Add(request);
                m_Event.Set();
            }
        }

        private void OnQueuedReadThreadExecute()
        {
            try
            {
                Trace.TraceInformation("QueuedReadThread started: {0}", m_DatabaseInstanceID);

                using (var stream = new BufferedStream(new FileStream(m_Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, FileOptions.RandomAccess), 1024 * 1024))
                {
                    //int handle;
                    WaitHandle[] handles = new WaitHandle[] { m_Quit, m_Event };

                    var pagedReader = new PagedStreamReader(stream, 4096);

                    while (WaitHandle.WaitAny(handles) == 1)// (handle = WaitHandle.WaitAny(handles)) == 0)
                    {
                        Thread.Sleep(200); //Wait to consolidate if more requests are coming in.

                        QueuedReadRequest[] requests = null;
                        lock (m_SyncObject)
                        {
                            if (m_Queue.Count > 0)
                            {
                                requests = m_Queue.ToArray();
                                m_Queue.Clear();
                            }
                            else
                                m_Event.Reset();
                        }

                        if (requests == null)
                            continue;

                        LinkedList<LogRowIndex> indicesList = new LinkedList<LogRowIndex>();
                        Dictionary<LogRowIndex, List<RequestPoints>> indexToRequestPoints = new Dictionary<LogRowIndex, List<RequestPoints>>();

                        foreach (var readRequest in requests)
                        {
                            var indices = m_Indexes.GetItemRows(readRequest.ItemID, readRequest.StartIndex, readRequest.Count);

                            var requestPoints = new RequestPoints(readRequest);

                            foreach (var logRowIndex in indices)
                            {
                                if (!indexToRequestPoints.TryGetValue(logRowIndex, out var list))
                                {
                                    list = new List<RequestPoints>();
                                    indexToRequestPoints.Add(logRowIndex, list);
                                }
                                list.Add(requestPoints);
                            }

                            indicesList.AddSorted(indices);
                        }



                        foreach (var logRowIndex in indicesList)
                        {
                            var point = LogRowDataFactory.CreateDataPoint(pagedReader, logRowIndex);

                            var list = indexToRequestPoints[logRowIndex];
                            foreach (var rp in list)
                            {
                                if (rp.Add(point))
                                {
                                    rp.Send(m_DatabaseInstanceID);
                                }
                            }
                        }

                        foreach (var index in indexToRequestPoints)
                        {
                            index.Value.ForEach(points => points.Send(m_DatabaseInstanceID));
                        }
                    }
                }
                Trace.TraceInformation("QueuedReadThread ended: {0}", m_DatabaseInstanceID);
            }
            catch (Exception ex)
            {
                m_Log.Fatal(ex.Message, ex);
                throw;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            m_Quit.Set();
            m_QueuedReadThread.Join();
            Trace.TraceInformation("QueuedReadThread disposed: {0}", m_DatabaseInstanceID);

            m_Queue.Clear();
        }

        #endregion
    }

    internal class RequestPoints
    {
        private readonly QueuedReadRequest m_request;
        private List<LogRowDataPoint> m_dataPoints = new List<LogRowDataPoint>();

        public RequestPoints(QueuedReadRequest request)
        {
            m_request = request;
        }

        /// <summary>
        /// Returns true if request is full filled
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Add(LogRowDataPoint point)
        {
            m_dataPoints.Add(point);

            return m_dataPoints.Count == m_request.BatchSize;
        }

        public void Send(long instanceId)
        {
            if (m_dataPoints.Count == 0)
                return;

            m_request.SendResult(instanceId, m_dataPoints.ToArray());
            m_dataPoints.Clear();
        }
    }

    public static class LinkedListExtensions
    {
        public static void AddSorted<T>(this LinkedList<T> target, IEnumerable<T> source) where T : IComparable<T>
        {
            var node = target.First;

            foreach (var sourceValue in source)
            {
                while (node != null && sourceValue.CompareTo(node.Value) > 0)
                {
                    node = node.Next;
                }

                if (node != null)
                    target.AddBefore(node, sourceValue);
                else
                    target.AddLast(sourceValue);
            }
        }
    }
}
