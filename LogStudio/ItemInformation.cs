using LogStudio.Data;
using System;

namespace LogStudio
{
    public class ItemInformation
    {
        public int Count;
        public double MaxValue = double.MinValue;
        public DateTime MaxValueTimeStamp;

        public double MinValue = double.MaxValue;
        public DateTime MinValueTimeStamp;

        public string ItemId { get; }

        private readonly DateTime m_From;
        private readonly DateTime m_To;

        public ItemInformation(string itemId, DateTime from, DateTime to)
        {
            ItemId = itemId;
            m_From = from;
            m_To = to;
        }

        public void InfoCollect(LogRowDataPoint row)
        {
            if (row.TimeStamp < m_From || row.TimeStamp > m_To)
                return;

            Count++;

            if (row.Value > MaxValue)
            {
                MaxValue = row.Value;
                MaxValueTimeStamp = row.TimeStamp;
            }

            if (row.Value < MinValue)
            {
                MinValue = row.Value;
                MinValueTimeStamp = row.TimeStamp;
            }
        }
    }
}