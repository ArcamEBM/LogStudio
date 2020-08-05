using System;
using System.Globalization;
using System.IO;

namespace LogStudio.Data
{
    public static class LogRowDataFactory
    {
        private static readonly CultureInfo m_Culture = new CultureInfo("en-US");

        internal static LogRowDataPoint CreateDataPoint(PagedStreamReader reader, LogRowIndex index)
        {
            byte[] buffer = new byte[index.RowLength];
            char[] chars = new char[index.RowLength];

            reader.Read(buffer, index.RowStart, index.RowLength);

            int length = buffer.IndexOf((byte)'\r', 0);
            if (length == -1)
                length = buffer.IndexOf((byte)'\n', 0);

            for (int i = 0; i < length; i++)
            {
                chars[i] = (char)buffer[i];
            }

            index.GetValues(out int timeStampIndex, out int nameIndex, out int userIndex, out int cycleIndex, out int valueIndex);

            var timeStamp = ParseDate(chars, 0, LogRowIndex.TimeStampLength); //, "yyyy-MM-dd hh:mm:ss.fff", culture);

            string valueString = new string(chars, valueIndex, length - valueIndex);

            double value;

            if (!double.TryParse(valueString, NumberStyles.Any, m_Culture, out double val))
            {
                value = valueString == "True" ? 1d : 0d;
            }
            else
            {
                value = val;
            }

            return new LogRowDataPoint(timeStamp, value);
        }

        internal static LogRowData CreateData(Stream stream, LogRowIndex index)
        {
            byte[] buffer = new byte[index.RowLength + 2];
            char[] chars = new char[index.RowLength];

            if (stream.Position != index.RowStart)
                stream.Seek(index.RowStart, SeekOrigin.Begin);

            stream.Read(buffer, 0, index.RowLength);

            int length = buffer.IndexOf((byte)'\r', 0);
            if (length == -1)
                length = buffer.IndexOf((byte)'\n', 0);

            for (int i = 0; i < length; i++)
            {
                chars[i] = (char)buffer[i];
            }

            index.GetValues(out int timeStampIndex, out int nameIndex, out int userIndex, out int cycleIndex, out int valueIndex);

            var itemId = new string(chars, nameIndex, index.NameLength);
            var timeStamp = LogRowDataFactory.ParseDate(chars, 0, LogRowIndex.TimeStampLength);
            var user = new string(chars, userIndex, index.UserLength);
            var cycleIndexValue = long.Parse(new string(chars, cycleIndex, index.CycleLength), NumberStyles.None);
            var value = new string(chars, valueIndex, length - valueIndex);

            return new LogRowData(timeStamp, itemId, user, cycleIndexValue, value);
        }

        private static DateTime ParseDate(char[] chars, int start, int length)
        {
            int year = ParseUInt(chars, start, 4);
            start += 5;
            int month = ParseUInt(chars, start, 2);
            start += 3;
            int day = ParseUInt(chars, start, 2);
            start += 3;
            int hour = ParseUInt(chars, start, 2);
            start += 3;
            int min = ParseUInt(chars, start, 2);
            start += 3;
            int s = ParseUInt(chars, start, 2);
            start += 3;
            int ms = ParseUInt(chars, start, 3);

            return new DateTime(year, month, day, hour, min, s, ms);
        }

        private static int ParseUInt(char[] c, int start, int length)
        {
            const int zero = (int)'0';

            int res = 0;


            for (int i = start; i < start + length; i++)
            {
                if (i > 0)
                    res *= 10;
                res += c[i] - zero;
            }

            return res;
        }
    }
}
