using System;
using System.Collections.Generic;

namespace LogStudio.Data
{
    internal class RowIndexCollection
    {
        internal readonly List<LogRowIndex> Indexes = new List<LogRowIndex>();
        internal readonly Dictionary<string, List<LogRowIndex>> IndexesByItem = new Dictionary<string, List<LogRowIndex>>(StringComparer.Ordinal);

        public int Count;

        internal void Add(string item, LogRowIndex index)
        {
            List<LogRowIndex> indexByItem;
            if (!IndexesByItem.TryGetValue(item, out indexByItem))
            {
                indexByItem = new List<LogRowIndex>();
                IndexesByItem.Add(item, indexByItem);
            }

            indexByItem.Add(index);
            Indexes.Add(index);
            Count++;
        }
    }
}
