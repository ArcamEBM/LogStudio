using LogStudio.Data;
using NUnit.Framework;
using System;
using System.Data;
using System.IO;
using System.Threading;

namespace LogStudio.DataTest
{
    [TestFixture]
    public class DataTest
    {
        [Test]
        public void TestSimpleExport()
        {
            string filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Testfiles/TestLog2.plg");
            LogDatabase db = GetDataBaseFromFile(filename);

            DataTable table = db.ExportStateOnItemChangeToDataTable("Alpha");
            Assert.AreEqual(table.Rows.Count, 2);
            Assert.AreEqual(table.Columns.Count, 3);

            Assert.AreEqual(table.Rows[0][0].ToString(), "False");
            Assert.AreEqual(table.Rows[1][0].ToString(), "True");

            Assert.AreEqual(table.Rows[0][1].ToString(), "20");
            Assert.AreEqual(table.Rows[1][1].ToString(), "22");

            Assert.AreEqual(table.Rows[0][2].ToString(), "0.7");
            Assert.AreEqual(table.Rows[1][2].ToString(), "0.7");
        }


        [Test]
        public static void TestConvertTime()
        {
            string time1 = "2011-11-18 14:35:17.781";
            string time2 = "2011-11-18 14:35:17.700";
            string res1 = ConvertDateToTime(time1, new DateTime(2011, 1, 1));
            string res2 = ConvertDateToTime(time2, new DateTime(2011, 1, 1));
        }

        private static string ConvertDateToTime(string p, DateTime dateTime)
        {
            DateTime t = DateTime.Parse(p);

            return ((ulong)(t.Ticks * 0.0001 - dateTime.Ticks * 0.0001)).ToString();
        }

        public static LogDatabase GetDataBaseFromFile(string filename)
        {
            filename = Path.GetFullPath(filename);
            Assert.That(File.Exists(filename));

            LogDatabase db = new LogDatabase(filename, false, null);
            db.Start();
            while (db.State != ProgressStateEnum.Done)
            {
                Thread.Sleep(10);
            }

            return db;
        }
    }
}
