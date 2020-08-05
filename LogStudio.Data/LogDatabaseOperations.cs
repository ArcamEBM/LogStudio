using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace LogStudio.Data
{
    public partial class LogDatabase
    {
        public void ExportToCSV(string filename, string itemID, char separator)
        {
            CultureInfo culture = Thread.CurrentThread.CurrentCulture;

            using (var writer = new StreamWriter(filename))
            {
                writer.WriteLine(
                    "{0}{1}{2}",
                    ConverToCSVField(
                        Properties.Resources.RES_ExportToCSVLabelTimeStamp,
                        separator),
                    separator,
                    ConverToCSVField(
                        Properties.Resources.RES_ExportToCSVLabelValue,
                        separator));

                foreach (LogRowData point in GetAll(itemID))
                {
                    writer.WriteLine(
                        "{0}{1}{2}",
                        ConverToCSVField(
                            point.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                            separator),
                        separator,
                        ConverToCSVField(
                            point.Value.Replace(".", culture.NumberFormat.CurrencyDecimalSeparator),
                            separator));
                }
            }
        }

        public void ExportStateOnItemChangeToCSV(string filename, string keyItem)
        {
            DataTable table = ExportStateOnItemChangeToDataTable(keyItem);

            string csv = CsvWriter.WriteToString(table, true);
            var t = new FileInfo(filename);
            using (StreamWriter tex = t.CreateText())
            {
                tex.Write(csv);
            }
        }

        public DataTable ExportStateOnItemChangeToDataTable(string keyItem)
        {
            //find all changes at the keyItem
            //Create an array with all parameter names in it
            //Create an array of List where the array index match the parameter name
            //and list index match the parameter value at the keyItem change
            LogRowData[] keyItemRows = GetItemRows(keyItem);
            string[] itemIDs = GetItemsIDs().Where(p => p != keyItem).ToArray();
            var result = new string[keyItemRows.Length, itemIDs.Length];

            for (var item = 0; item < itemIDs.Length; item++)
            {
                LogRowData[] allData = GetItemRows(itemIDs[item]);
                var allDataIndex = 0;
                int maxIndex = allData.Length;
                for (var i = 0; i < keyItemRows.Length; i++)
                {
                    long timeStamp = keyItemRows[i].CycleIndex;
                    while (allDataIndex < maxIndex && allData[allDataIndex].CycleIndex < timeStamp)
                        allDataIndex++;

                    LogRowData lastItem = allData[Math.Min(allDataIndex, maxIndex - 1)];
                    result[i, item] = lastItem.Value;
                }
            }

            //Save to csv
            var table = new DataTable();
            table.Columns.Add(keyItem, typeof(string));
            foreach (string itemID in itemIDs)
            {
                table.Columns.Add(itemID, typeof(string));
            }

            for (var i = 0; i < result.GetLength(0); i++)
            {
                var row = new object[result.GetLength(1) + 1];
                row[0] = keyItemRows[i].Value;
                for (var j = 0; j < row.Length - 1; j++)
                {
                    row[j + 1] = result[i, j];
                }

                table.Rows.Add(row);
            }

            return table;
        }

        private static string ConverToCSVField(string data, char separator)
        {
            if (data.Contains(separator))
                return $"\"{data}\"";
            return data;
        }
    }
}