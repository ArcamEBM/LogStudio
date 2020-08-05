using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LogStudio.Data
{
    internal class LogIndexCollection
    {
        public event EventHandler<LogIndexChangesEventArgs> OnIndexesChanged;
        public event EventHandler<LogIndexAddedEventArgs> OnIndexesAdded;

        private readonly List<LogRowIndex> m_LogRowIndexes = new List<LogRowIndex>();

        private readonly Dictionary<string, List<LogRowIndex>> m_RowIndexes;
        private readonly Dictionary<string, ItemProperties> m_ItemProperties = new Dictionary<string, ItemProperties>(StringComparer.Ordinal);

        private readonly List<string> m_AddedIndexes = new List<string>();
        private readonly List<LogIndexChangeRange> m_ChangedIndexes = new List<LogIndexChangeRange>();

        private readonly ReaderWriterLockSlim m_Lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private readonly StringComparer m_Comparer = StringComparer.Ordinal;

        public LogIndexCollection()
        {
            m_RowIndexes = new Dictionary<string, List<LogRowIndex>>();
        }

        public int Count
        {
            get { return m_LogRowIndexes.Count; }
        }

        public void AddIndexs(RowIndexCollection indexCollection)
        {
            m_AddedIndexes.Clear();
            m_ChangedIndexes.Clear();

            m_Lock.EnterWriteLock();
            try
            {
                if (m_RowIndexes.Count == 0)
                {
                    foreach (KeyValuePair<string, List<LogRowIndex>> index in indexCollection.IndexesByItem)
                    {
                        m_RowIndexes.Add(index.Key, index.Value);
                        m_ChangedIndexes.Add(new LogIndexChangeRange(index.Key, 0, index.Value.Count));
                    }

                    m_AddedIndexes.AddRange(indexCollection.IndexesByItem.Keys);
                }
                else
                {
                    List<LogRowIndex> itemIndex;
                    foreach (KeyValuePair<string, List<LogRowIndex>> index in indexCollection.IndexesByItem)
                    {
                        if (m_RowIndexes.TryGetValue(index.Key, out itemIndex))
                        {
                            m_ChangedIndexes.Add(new LogIndexChangeRange(index.Key, itemIndex.Count, index.Value.Count));
                            itemIndex.AddRange(index.Value);
                        }
                        else
                        {
                            m_ChangedIndexes.Add(new LogIndexChangeRange(index.Key, 0, index.Value.Count));
                            m_RowIndexes.Add(index.Key, index.Value);
                            m_AddedIndexes.Add(index.Key);
                        }
                    }
                }

                m_LogRowIndexes.AddRange(indexCollection.Indexes);
            }
            finally
            {
                m_Lock.ExitWriteLock();
            }

            if (m_AddedIndexes.Count > 0)
            {
                if (OnIndexesAdded != null)
                    OnIndexesAdded(this, new LogIndexAddedEventArgs(m_AddedIndexes.ToArray()));
            }

            if (m_ChangedIndexes.Count > 0)
            {
                if (OnIndexesChanged != null)
                    OnIndexesChanged(this, new LogIndexChangesEventArgs(m_ChangedIndexes.ToArray()));
            }
        }

        public void AddProperties(ItemProperties properties)
        {
            m_Lock.EnterWriteLock();
            try
            {
                m_ItemProperties[properties["Name"]] = properties;
            }
            finally
            {
                m_Lock.ExitWriteLock();
            }
        }

        public ItemProperties GetItemProperties(string fullname)
        {
            m_Lock.EnterReadLock();
            try
            {
                ItemProperties properties;
                if (m_ItemProperties.TryGetValue(fullname, out properties))
                    return properties;

                return null;
            }
            finally
            {
                m_Lock.ExitReadLock();
            }
        }

        public IEnumerable<string> GetItemsIDs()
        {
            string[] itemIDs;

            m_Lock.EnterReadLock();
            try
            {
                itemIDs = m_RowIndexes.Keys.ToArray();
            }
            finally
            {
                m_Lock.ExitReadLock();
            }

            if (itemIDs != null)
            {
                foreach (string itemID in itemIDs)
                {
                    yield return itemID;
                }
            }
        }

        public LogRowIndex[] GetItemRows(string itemID)
        {
            m_Lock.EnterReadLock();
            try
            {
                return m_RowIndexes[itemID].ToArray();
            }
            finally
            {
                m_Lock.ExitReadLock();
            }
        }

        public LogRowIndex[] FindIndices(string itemId, Predicate<LogRowIndex> query)
        {
            m_Lock.EnterReadLock();
            try
            {
                return m_RowIndexes[itemId].FindAll(query).ToArray();
            }
            finally
            {
                m_Lock.ExitReadLock();
            }
        }


        public LogRowIndex[] GetItemRows(string itemID, int fromIndex, int length)
        {
            m_Lock.EnterReadLock();
            try
            {
                LogRowIndex[] res = new LogRowIndex[length];

                List<LogRowIndex> items = m_RowIndexes[itemID];

                length = Math.Min(length, items.Count);

                for (int index = 0; index < length; index++)
                {
                    res[index] = items[fromIndex + index];
                }
                return res;
            }
            finally
            {
                m_Lock.ExitReadLock();
            }
        }

        public int GetItemRowCount(string itemID)
        {
            m_Lock.EnterReadLock();
            try
            {
                if (m_RowIndexes.TryGetValue(itemID, out var rows))
                    return rows.Count;

                return 0;
            }
            finally
            {
                m_Lock.ExitReadLock();
            }
        }

        public LogRowIndex GetItemRow(string itemID, int index)
        {
            m_Lock.EnterReadLock();
            try
            {
                return m_RowIndexes[itemID][index];
            }
            finally
            {
                m_Lock.ExitReadLock();
            }
        }

        public bool Exists(string itemID)
        {
            m_Lock.EnterReadLock();
            try
            {
                return m_RowIndexes.ContainsKey(itemID);
            }
            finally
            {
                m_Lock.ExitReadLock();
            }
        }

        internal LogRowIndex GetFirstItem()
        {
            m_Lock.EnterReadLock();
            try
            {
                return m_LogRowIndexes[0];
            }
            finally
            {
                m_Lock.ExitReadLock();
            }
        }

        internal LogRowIndex GetLastItem()
        {
            m_Lock.EnterReadLock();
            try
            {
                return m_LogRowIndexes[m_LogRowIndexes.Count - 1];
            }
            finally
            {
                m_Lock.ExitReadLock();
            }
        }

        internal LogRowIndex GetLastIndex(string itemID)
        {
            m_Lock.EnterReadLock();
            try
            {
                List<LogRowIndex> list = m_RowIndexes[itemID];
                return list[list.Count - 1];
            }
            finally
            {
                m_Lock.ExitReadLock();
            }
        }

        public LogRowIndex[] GetCompleteIndexList()
        {
            m_Lock.EnterReadLock();
            try
            {
                return m_LogRowIndexes.ToArray();
            }
            finally
            {
                m_Lock.ExitReadLock();
            }
        }

        internal LogRowIndex[] GetItemRows(int startIndex, int count)
        {
            m_Lock.EnterReadLock();
            try
            {
                return m_LogRowIndexes.GetRange(startIndex, count).ToArray();
            }
            finally
            {
                m_Lock.ExitReadLock();
            }
        }
    }
}