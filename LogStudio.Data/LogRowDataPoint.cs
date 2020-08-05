using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LogStudio.Data
{
    [DebuggerDisplay("{TimeStamp}, {Value}")]
    public class LogRowDataPoint : LogRow, IEqualityComparer<LogRowDataPoint>
    {
        public double Value { get; private set; }

        public LogRowDataPoint()
        {
        }

        public LogRowDataPoint(DateTime timestamp, double value)
            : base(timestamp)
        {
            Value = value;
        }

        public LogRowDataPoint(double seconds, double value)
            : base(DateTime.MinValue.AddSeconds(seconds))
        {
            Value = value;
        }

        public LogRowDataPoint(LogRowData data, IFormatProvider formatProvider)
        {
            TimeStamp = data.TimeStamp;
            Value = double.Parse(data.Value, formatProvider);
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", TimeStamp, Value);
        }


        #region IEqualityComparer<LogRowDataPoint> Members

        public bool Equals(LogRowDataPoint x, LogRowDataPoint y)
        {
            return x.TimeStamp == y.TimeStamp && x.Value == y.Value;
        }

        public int GetHashCode(LogRowDataPoint obj)
        {
            return obj.TimeStamp.GetHashCode();
        }

        #endregion
    }
}
