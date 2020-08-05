using LogStudio.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LogStudio.DataTest
{
    [TestFixture]
    unsafe public class LogRowDataTests
    {
        [Test]
        public void LogRowData()
        {
            const string row = "2011-01-24 06:29:46.687|Core.State.StartTime|Core|181188615|2010-12-03 09:43:03\r\n";

            string itemID;
            LogRowIndex index;

            char[] ptr = row.ToCharArray();

            LogStudio.Data.LogFileParser.ParseRow(ptr, row.Length, 0, '|', out index, out itemID);

            LogRowData data;

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(row)))
            {
                data = LogRowDataFactory.CreateData(stream, index);
            }

            Assert.AreEqual(181188615, data.CycleIndex);
            Assert.AreEqual("Core", data.User);
            Assert.AreEqual(DateTime.Parse("2011-01-24 06:29:46.687"), data.TimeStamp);
            Assert.AreEqual("2010-12-03 09:43:03", data.Value);
        }

        [Test]
        public void LogRowDataPoint()
        {
            const string row = "2011-01-24 06:29:46.687|Core.State.StartTime|Core|181188615|356.3245\r\n";

            string itemID;
            LogRowIndex index;

            char[] ptr = row.ToCharArray();
            LogStudio.Data.LogFileParser.ParseRow(ptr, row.Length, 0, '|', out index, out itemID);

            LogRowDataPoint data;

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(row)))
            {
                var pagedReader = new PagedStreamReader(stream, 4096);
                data = LogRowDataFactory.CreateDataPoint(pagedReader, index);
            }

            Assert.AreEqual(DateTime.Parse("2011-01-24 06:29:46.687"), data.TimeStamp);
            Assert.AreEqual(356.3245, data.Value);
        }

        [Test]
        public void LogRowDataPointRow2()
        {
            const string row = "2011-01-24 06:29:46.687|Core.State.StartTime|Core|181188615|356.3245\r\n" +
                               "2011-01-25 07:23:43.234|Core.State.StartTime|Core|181192343|3\r\n";

            long startPos = "2011-01-24 06:29:46.687|Core.State.StartTime|Core|181188615|356.3245\r\n".Length;

            string itemID;
            LogRowIndex index;

            char[] ptr = row.ToCharArray();
            LogStudio.Data.LogFileParser.ParseRow(ptr, "2011-01-25 07:23:43.234|Core.State.StartTime|Core|181192343|3\r\n".Length, startPos, '|', out index, out itemID);

            LogRowDataPoint data;

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(row)))
            {
                var pagedReader = new PagedStreamReader(stream, 4096);
                data = LogRowDataFactory.CreateDataPoint(pagedReader, index);
            }

            Assert.AreEqual(DateTime.Parse("2011-01-25 07:23:43.234"), data.TimeStamp);
            Assert.AreEqual(double.Parse("3"), data.Value);
        }

        [Test]
        public void ParseBufferTest()
        {
            const string data = "2011-01-24 06:29:46.687|Core.State.StartTime|Core|181188615|356.3245\r\n" +
                               "2011-01-25 07:23:43.234|Core.State.StartTime|Core|181192343|3\r\n";

            byte[] buffer = Encoding.UTF8.GetBytes(data);

            RowIndexCollection indexes = new RowIndexCollection();

            LogFileParser parser = new LogFileParser();

            parser.ParseBuffer(buffer, buffer.Length, indexes);

            List<LogRowIndex> rows = indexes.IndexesByItem["Core.State.StartTime"];

            IEnumerator<LogRowIndex> rowIndexes = rows.GetEnumerator();

            Assert.That(rowIndexes.MoveNext());

            LogRowIndex idx = rowIndexes.Current;

            Assert.AreEqual(0, idx.RowStart);
            Assert.AreEqual(70, idx.RowLength);
            Assert.AreEqual(20, idx.NameLength);
            Assert.AreEqual(4, idx.UserLength);
            Assert.AreEqual(9, idx.CycleLength);

            Assert.That(rowIndexes.MoveNext());

            idx = rowIndexes.Current;

            Assert.AreEqual(70, idx.RowStart);
            Assert.AreEqual(63, idx.RowLength);
            Assert.AreEqual(20, idx.NameLength);
            Assert.AreEqual(4, idx.UserLength);
            Assert.AreEqual(9, idx.CycleLength);

            Assert.That(!rowIndexes.MoveNext());
        }

    }
}
