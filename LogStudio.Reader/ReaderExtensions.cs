using JetBrains.Annotations;
using LogStudio.Data;
using System.Collections.Generic;
using System.Linq;

namespace LogStudio.Reader
{
    public static class ReaderExtensions
    {
        public static IEnumerable<LogRowKey> FindClosestTriggerKeys(this IItemDatabase db, long sourceUniqueId, long sourceCycleIndex, IEnumerable<string> triggerIds)
        {
            var triggerWithUniqueIds = triggerIds.Select(p => new { ItemId = p, UniqueIds = db.GetLogRowUniqueID(p) });

            foreach (var triggerWithUniqueId in triggerWithUniqueIds)
            {
                yield return db.FindClosestRowKey(sourceUniqueId, sourceCycleIndex, triggerWithUniqueId.ItemId, triggerWithUniqueId.UniqueIds);
            }
        }

        [CanBeNull]
        public static LogRowKey FindClosestRowKey(this IItemDatabase db, long sourceUniqueId, long sourceCycleIndex, string itemId, long[] uniqueIds)
        {
            for (int i = uniqueIds.Length - 1; i >= 0; i--)
            {
                if (uniqueIds[i] >= sourceUniqueId)
                    continue;

                LogRowData row = db.GetItemRow(itemId, uniqueIds[i]);
                if (row.CycleIndex == sourceCycleIndex && row.HasExpression())
                    continue;

                return new LogRowKey(uniqueIds[i], db.GetItemRow(itemId, uniqueIds[i]));
            }

            return null;
        }

        public static bool HasExpression(this LogRowData source)
        {
            return source.User.Contains("[");
        }
    }
}