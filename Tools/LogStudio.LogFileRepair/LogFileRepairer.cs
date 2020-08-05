using System.IO;
using System.Text.RegularExpressions;

namespace LogStudio.LogFileRepair
{
    public class LogFileRepairer
    {
        private const string RowValidation = @"^\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}.\d{3}\|[A-Za-z0-9\.\[\]_\s\(\)]+\|[^|]+\|\d+\|[\da-zA-z\.\-\s\:]*$";
        private string m_Filename;
        public volatile int OkRows = 0;
        public volatile int ErrorRows = 0;

        public LogFileRepairer(string filename)
        {
            m_Filename = filename;
        }

        public void Execute()
        {
            string dir = Path.GetDirectoryName(m_Filename);
            string file = Path.GetFileNameWithoutExtension(m_Filename);
            string repairedFile = Path.Combine(dir, file) + "_repaired.plg";
            string errorFile = Path.Combine(dir, file) + "_errors.plg";

            using (StreamReader sourceFile = new StreamReader(m_Filename))
            {
                using (StreamWriter targetFile = new StreamWriter(repairedFile))
                {
                    using (StreamWriter errFile = new StreamWriter(errorFile))
                    {
                        RepairFile(sourceFile, targetFile, errFile);
                    }
                }
            }
        }

        private void RepairFile(StreamReader sourceFile, StreamWriter targetFile, StreamWriter errFile)
        {
            while (!sourceFile.EndOfStream)
            {
                string row = sourceFile.ReadLine();
                if (row[0] == '#' || Regex.IsMatch(row, RowValidation, RegexOptions.Compiled | RegexOptions.Singleline))
                {
                    targetFile.WriteLine(row);
                    OkRows++;
                }
                else
                {
                    errFile.WriteLine(row);
                    ErrorRows++;
                }
            }
        }
    }
}
