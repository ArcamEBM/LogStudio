using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LogStudio.Data
{
    public class CSVParser
    {
        public static List<LogRowDataPoint> Parse(string filename, char separator)
        {
            List<LogRowDataPoint> points = new List<LogRowDataPoint>();

            using (StreamReader reader = new StreamReader(filename))
            {
                StringBuilder col = new StringBuilder();

                bool inQuota = false;
                while (!reader.EndOfStream)
                {
                    DateTime timestamp = DateTime.MinValue;
                    double value;
                    string row = reader.ReadLine();

                    for (int index = 0; index < row.Length; index++)
                    {
                        char c = row[index];

                        if (c == '\"')
                        {
                            inQuota = !inQuota;
                        }
                        else
                        {
                            if (!inQuota && c == separator)
                            {
                                timestamp = DateTime.Parse(col.ToString());
                                col = new StringBuilder();
                            }
                            else
                            {
                                col.Append(c);
                            }
                        }

                    }

                    value = double.Parse(col.ToString());

                    points.Add(new LogRowDataPoint(timestamp, value));
                }
            }
            return points;
        }
    }
}
