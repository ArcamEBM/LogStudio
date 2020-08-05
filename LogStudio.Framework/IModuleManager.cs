using log4net;
using System;

namespace LogStudio.Framework
{
    /// <summary>
    /// Module manager interface
    /// </summary>
    public interface IModuleManager
    {
        /// <summary>
        /// Logger to be used to send log messages to the module manager
        /// </summary>
        ILog Logger { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsAdmin { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="module"></param>
        /// <param name="selectedItems"></param>
        /// <param name="timeStamp"></param>
        void SendTo(IModule module, string[] selectedItems, DateTime timeStamp);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IModule[] GetModules();
    }
}
