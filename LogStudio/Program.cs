using log4net;
using LogStudio.Framework;
using System;
using System.Windows.Forms;

namespace LogStudio
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //System.Diagnostics.Debugger.Launch();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            log4net.Config.XmlConfigurator.Configure();//new FileInfo(Path.ChangeExtension(Application.ExecutablePath, ".Log4Net")));

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += OnThreadException;

            Application.Run(new Form1());
        }

        static void OnThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            LogManager.GetLogger("AppLog").Error("Unhandled Thread Exception", e.Exception);
            ExceptionBox.Show(e.Exception);
        }
    }
}
