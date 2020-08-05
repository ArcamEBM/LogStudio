using log4net;
using LogStudio.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace LogStudio
{
    public class ModuleManager : IModuleManager, IDisposable, INotifyPropertyChanged, IComparer<IModule>
    {
        private readonly ToolStrip m_ToolStrip;
        private ToolStripItem[] m_MenuItems;

        public event PropertyChangedEventHandler OnModulePropertyChanged;

        public ModuleManager(ILog logger, bool isAdmin, ToolStrip toolStrip)
        {
            m_ToolStrip = toolStrip;
            Logger = logger;
            Modules = new List<IModule>();
            IsAdmin = isAdmin;

            if (Directory.Exists(Properties.Settings.Default.UserStoragePath))
            {
                UserRootPath = Properties.Settings.Default.UserStoragePath;
            }
            else
            {
                UserRootPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                UserRootPath = Path.Combine(UserRootPath, "Log Studio");
                Directory.CreateDirectory(UserRootPath);

                Properties.Settings.Default.UserStoragePath = UserRootPath;
                Properties.Settings.Default.Save();
            }
        }

        public void LoadPlugins()
        {
            ModulesPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            string[] moduleFiles = Directory.GetFiles(ModulesPath, "LogStudio.Module.*.dll");

            foreach (string file in moduleFiles)
            {
                Assembly assembly = Assembly.LoadFile(file);
                try
                {
                    Type[] types = assembly.GetExportedTypes();

                    foreach (Type type in types)
                    {
                        if (type.GetInterface(typeof(IModule).Name) != null)
                        {
                            try
                            {
                                var module = (IModule)Activator.CreateInstance(type);
                                Modules.Add(module);
                            }
                            catch
                            {
                                //TODO Log exceptions 
                            }
                        }
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                }

                foreach (IModule module in Modules)
                {
                    module.Initialize(this);
                }

                Modules.Sort(this);
            }

            if (Modules.Count > 0)
                SelectedModule = Modules[0];
        }

        private string ModulesPath { get; set; }

        public List<IModule> Modules { get; }

        private IModule m_SelectedModule;

        public bool IsAdmin { get; }
        /// <summary>
        /// Contains the currently selected module
        /// </summary>
        public IModule SelectedModule
        {
            get => m_SelectedModule;
            set
            {
                m_ToolStrip.SuspendLayout();

                try
                {
                    if (m_SelectedModule != null)
                    {
                        m_SelectedModule.PropertyChanged -= OnModulePropertyChanged;
                        if (m_MenuItems != null)
                        {
                            foreach (ToolStripItem item in m_MenuItems)
                            {
                                m_ToolStrip.Items.Remove(item);
                            }
                        }
                        m_SelectedModule.Hidden();
                    }

                    m_SelectedModule = value;

                    if (m_SelectedModule != null)
                    {
                        m_SelectedModule.Shown(out m_MenuItems);

                        if (m_MenuItems != null)
                        {
                            foreach (ToolStripItem item in m_MenuItems)
                            {
                                m_ToolStrip.Items.Add(item);
                            }
                        }

                        m_SelectedModule.PropertyChanged += OnModulePropertyChanged;
                    }
                }
                finally
                {
                    m_ToolStrip.ResumeLayout(true);
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedModule"));
            }
        }


        #region IModuleManager Members

        public ILog Logger
        {
            get;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Modules.Clear();
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IComparer<IModule> Members

        int IComparer<IModule>.Compare(IModule x, IModule y)
        {
            return y.Priority.CompareTo(x.Priority);
        }

        #endregion

        #region IModuleManager Members

        private string UserRootPath
        {
            get;
        }

        #endregion


        #region IModuleManager Members


        public void SendTo(IModule module, string[] selectedItems, DateTime timeStamp)
        {
            if (module.CheckedItems != null)
            {
                if (module.TreeVisible && selectedItems != null)
                {
                    foreach (string itemId in selectedItems)
                    {
                        if (!string.IsNullOrEmpty(itemId) && !module.CheckedItems.Contains(itemId))
                            module.CheckedItems.Add(itemId);
                    }
                }
            }

            module.GotoTime(string.Empty, timeStamp);

            SelectedModule = module;
        }

        public IModule[] GetModules()
        {
            return Modules.ToArray();
        }

        #endregion
    }
}