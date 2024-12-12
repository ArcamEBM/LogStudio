using LogStudio.Data;
using NUnit.Framework;
using System.Collections.Generic;

namespace LogStudio.Data.Test
{
    [TestFixture]
    public class LogFileParserTest
    {
        [Test]
        public void ParseRowTest()
        {
            Dictionary<string, List<LogRowIndex>> indexes = new Dictionary<string, List<LogRowIndex>>();

            const string row = "2011-01-24 06:29:46.687|Core.State.StartTime|Core|181188615|2010-12-03 09:43:03\r\n";

            string itemID;
            LogRowIndex index;

            char[] ptr = row.ToCharArray();
            LogStudio.Data.LogFileParser.ParseRow(ptr, row.Length, 0, '|', out index, out itemID);

            Assert.AreEqual("Core.State.StartTime", itemID);
            Assert.AreEqual(0, index.RowStart);
            Assert.AreEqual(row.Length, index.RowLength);
            Assert.AreEqual("Core.State.StartTime".Length, index.NameLength);
            Assert.AreEqual("181188615".Length, index.CycleLength);
            //Assert.AreEqual(, "2010-12-03 09:43:03".Length);
        }
    }
}
