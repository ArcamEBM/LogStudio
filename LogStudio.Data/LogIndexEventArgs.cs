using System;

namespace LogStudio.Data
{
    public class LogIndexAddedEventArgs : EventArgs
    {
        public LogIndexAddedEventArgs(string[] addedIndexes)
        {
            AddedIndexes = addedIndexes;
        }

        public string[] AddedIndexes { get; private set; }
    }

    public class LogIndexChangesEventArgs : EventArgs
    {
        public LogIndexChangesEventArgs(LogIndexChangeRange[] changes)
        {
            Changes = changes;
        }

        public LogIndexChangeRange[] Changes { get; private set; }
    }

    public class LogIndexChangeRange
    {
        public readonly string ItemID;
        public readonly int Index;
        public readonly int Count;

        public LogIndexChangeRange(string itemID, int index, int count)
        {
            ItemID = itemID;
            Index = index;
            Count = count;
        }
    }
}
