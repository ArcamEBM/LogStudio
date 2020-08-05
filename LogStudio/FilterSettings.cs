using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace LogStudio
{
    public class FilterSettings
    {
        public HashSet<string> AllowedItems;

        public void ToFile(string filename)
        {
            using (var stream = new GZipStream(File.Create(filename), CompressionMode.Compress))
            {
                var writer = new StreamWriter(stream);

                foreach (string item in AllowedItems)
                {
                    writer.WriteLine(item);
                }

                writer.Dispose();
            }
        }

        public static FilterSettings FromFile(string filename)
        {
            var settings = new FilterSettings();

            using (var stream = new GZipStream(File.OpenRead(filename), CompressionMode.Decompress))
            {
                var reader = new StreamReader(stream);

                while (!reader.EndOfStream)
                {
                    if (settings.AllowedItems == null)
                        settings.AllowedItems = new HashSet<string>();

                    settings.AllowedItems.Add(reader.ReadLine());
                }

                reader.Dispose();
            }

            return settings;
        }
    }
}