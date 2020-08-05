using System.Data;
using System.IO;

namespace LogStudio.Data
{
    public class CsvWriter
    {
        public static string WriteToString(DataTable table, bool header)
        {
            var writer = new StringWriter();
            WriteToStream(writer, table, header);
            return writer.ToString();
        }

        private static void WriteToStream(TextWriter stream, DataTable table, bool header)
        {
            if (header)
            {
                for (var i = 0; i < table.Columns.Count; i++)
                {
                    WriteItem(stream, table.Columns[i].Caption);
                    stream.Write(i < table.Columns.Count - 1 ? ';' : '\n');
                }
            }
            foreach (DataRow row in table.Rows)
            {
                for (var i = 0; i < table.Columns.Count; i++)
                {
                    WriteItem(stream, row[i]);
                    stream.Write(i < table.Columns.Count - 1 ? ';' : '\n');
                }
            }
        }

        private static void WriteItem(TextWriter stream, object item)
        {
            if (item == null)
                return;
            var s = item.ToString();
            if (s.IndexOfAny("\",\x0A\x0D;".ToCharArray()) > -1)
                stream.Write("\"" + s.Replace("\"", "\"\"") + "\"");
            else
                stream.Write(s);
        }
    }
}
