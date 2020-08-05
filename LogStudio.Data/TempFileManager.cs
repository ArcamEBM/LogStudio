using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace LogStudio.Data
{
    public class TempFileManager : Disposable
    {
        private List<string> m_TempFiles = new List<string>();

        public string GetTempFile()
        {
            string path = Path.GetTempFileName();
            m_TempFiles.Add(path);
            return path;
        }

        protected override void OnUnManagedDispose()
        {
            foreach (string file in m_TempFiles)
            {
                DateTime start = DateTime.UtcNow;
                while (DateTime.UtcNow.Subtract(start).Seconds < 30)
                {
                    try
                    {
                        if (File.Exists(file))
                            File.Delete(file);
                        break;
                    }
                    catch
                    {
                        Thread.CurrentThread.Join(10);
                    }
                }
            }
        }
    }
}
