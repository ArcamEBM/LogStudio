using System;

namespace LogStudio.Data
{
    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal class LogRowIndex : IComparable<LogRowIndex>
    {
        public const short TimeStampLength = 23;

        public readonly long RowStart;
        public readonly ushort RowLength;
        public readonly ushort NameLength;
        public readonly ushort UserLength;
        public readonly byte CycleLength;

        public LogRowIndex(long rowStart, ushort rowLength, ushort nameLength, ushort userLength, byte cycleLength)
        {
            RowStart = rowStart;
            RowLength = rowLength;
            NameLength = nameLength;
            UserLength = userLength;
            CycleLength = cycleLength;
        }

        public void GetValues(out int timeStampIndex, out int nameIndex, out int userIndex, out int cycleIndex, out int valueIndex)
        {
            timeStampIndex = 0;
            nameIndex = timeStampIndex + TimeStampLength + 1;
            userIndex = nameIndex + NameLength + 1;
            cycleIndex = userIndex + UserLength + 1;
            valueIndex = cycleIndex + CycleLength + 1;
        }

        public override int GetHashCode()
        {
            return RowStart.GetHashCode();
        }

        public int CompareTo(LogRowIndex other)
        {
            return RowStart.CompareTo(other.RowStart);
        }

        public override bool Equals(object obj)
        {
            if (obj is LogRowIndex index)
                return RowStart == index.RowStart;
            return base.Equals(obj);
        }
    }
}
