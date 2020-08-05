using LogStudio.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LogStudio.DataTest
{
    [TestFixture]
    public class LogDatabaseTests
    {
        [Test]
        public void GetCycleStateTest()
        {
            using (LogDatabase db = new LogDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Testfiles\\TestLog4.plg"), false, null))
            {
                db.Read();

                var data = db.GetCycleState(1805).ToArray();

                Assert.AreEqual(3, data.Length);
                Assert.NotNull(Array.Find(data, rowData => rowData.ItemID == "Alpha" && rowData.CycleIndex == 1669 && rowData.Value == "False"));
                Assert.NotNull(Array.Find(data, rowData => rowData.ItemID == "Beta" && rowData.CycleIndex == 1803 && rowData.Value == "24"));
                Assert.NotNull(Array.Find(data, rowData => rowData.ItemID == "Delta" && rowData.CycleIndex == 1669 && rowData.Value == "0.7"));
            }
        }

        [Test]
        public void GetAllDPTests()
        {
            using (LogDatabase db = new LogDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Testfiles\\TestLog4.plg"), false, null))
            {
                db.Read();

                var data = db.GetAllDP(new KeyValuePair<string, LogRowType>[] { new KeyValuePair<string, LogRowType>("Alpha", LogRowType.Raw), new KeyValuePair<string, LogRowType>("Delta", LogRowType.Point) }).ToArray();

                Assert.AreEqual("Alpha", data[0].Fullname);
                Assert.AreEqual("False", data[0].GetAsDataRow.Value);

                Assert.AreEqual("Delta", data[1].Fullname);
                Assert.AreEqual(0.7, data[1].GetAsDataPoint.Value, 0.1);

                Assert.AreEqual("Alpha", data[2].Fullname);
                Assert.AreEqual("True", data[2].GetAsDataRow.Value);

                Assert.AreEqual("Delta", data[3].Fullname);
                Assert.AreEqual(0.9, data[3].GetAsDataPoint.Value, 0.1);
            }
        }

        [Test]
        public void GetAllDPTestsOneExists()
        {
            using (LogDatabase db = new LogDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Testfiles\\TestLog4.plg"), false, null))
            {
                db.Read();

                NamedLogRow[] data;
                Assert.Catch<KeyNotFoundException>(() => data = db.GetAllDP(new[] { new KeyValuePair<string, LogRowType>("Gamma", LogRowType.Point), new KeyValuePair<string, LogRowType>("Delta", LogRowType.Raw) }).ToArray());
            }
        }
    }
}
