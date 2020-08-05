using System;
using System.IO;
using System.Threading;

namespace LogStudio.LogFileRepair
{
    class Program
    {
        private static volatile bool Done = false;

        static void Main(string[] args)
        {
            string file = args[0];

            if (!File.Exists(file))
            {
                Console.WriteLine("File not found!");
                return;
            }

            if (Path.GetExtension(file).ToLower() != ".plg")
            {
                Console.WriteLine("File has to be of plg type.!");
                return;
            }

            Console.WriteLine("Repairing file {0}", Path.GetFileNameWithoutExtension(file));
            Console.WriteLine();

            LogFileRepair.LogFileRepairer repairer = new LogFileRepairer(file);

            ThreadPool.QueueUserWorkItem(OnExecute, repairer);

            while (!Done)
            {
                Console.Write("Rows successfully read: {0}, Error rows: {1}\r", repairer.OkRows, repairer.ErrorRows);
                Thread.CurrentThread.Join(200);
            }

            Console.WriteLine();
            Console.WriteLine("Done.");
        }

        private static void OnExecute(object state)
        {
            LogFileRepairer repairer = (LogFileRepairer)state;

            try
            {
                repairer.Execute();
            }
            finally
            {
                Done = true;
            }
        }
    }
}
