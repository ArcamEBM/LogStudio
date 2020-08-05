using CommandLine;
using JetBrains.Annotations;
using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace LogFileGenerator
{
    static class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(options =>
                {
                    using (var source = new StreamReader(new FileStream(options.SourceFile, FileMode.Open, FileAccess.Read, FileShare.Read)))
                    using (var target = new StreamWriter(options.TargetFile != null ? new FileStream(options.TargetFile, FileMode.Create, FileAccess.Write, FileShare.Read) : Console.OpenStandardOutput()))
                    {
                        target.AutoFlush = true;

                        DateTime lastLineTimestamp = DateTime.MinValue;

                        NumberFormatInfo format = CultureInfo.InvariantCulture.NumberFormat;

                        while (!source.EndOfStream)
                        {
                            var line = source.ReadLine();
                            if (string.IsNullOrEmpty(line))
                                continue;

                            if (line[0] != '#')
                            {
                                DateTime timestamp = DateTime.ParseExact(line.Substring(0, 23), "yyyy-MM-dd HH:mm:ss.fff",
                                    format);

                                if (lastLineTimestamp != DateTime.MinValue)
                                {
                                    var delta = (int)(timestamp - lastLineTimestamp).TotalMilliseconds;

                                    if (delta > 0)
                                    {
                                        Thread.Sleep(delta);
                                    }
                                }

                                lastLineTimestamp = timestamp;
                            }

                            target.WriteLine(line);
                        }
                    }
                }
            );

            Console.ReadLine();
        }
    }

    [PublicAPI] // Serialized
    internal class Options
    {
        [Option('s', Required = true, HelpText = "Source file to generate target file from.")]
        public string SourceFile { get; set; }

        [Option('t', Required = false, HelpText = "Target file generated from source.")]
        public string TargetFile { get; set; }
    }
}