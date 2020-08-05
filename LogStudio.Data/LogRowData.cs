using System;

namespace LogStudio.Data
{
    public class LogRowData : LogRow
    {
        public readonly string ItemID;
        public readonly string User;
        public readonly long CycleIndex;
        public readonly string Value;

        public LogRowData(DateTime timeStamp, string itemID, string user, long cycleIndex, string value)
        : base(timeStamp)
        {
            ItemID = itemID;
            User = user;
            CycleIndex = cycleIndex;
            Value = value;
        }

        public double OaTimeStamp => TimeStamp.ToOADate();

        public override string ToString()
        {
            return $"{TimeStamp}, {User}, {CycleIndex}, {Value}";
        }
    }
}
