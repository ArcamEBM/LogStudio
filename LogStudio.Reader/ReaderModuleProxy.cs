using LogStudio.Framework;
using LogStudio.Reader.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace LogStudio.Reader
{
    
    // ReSharper disable once UnusedType.Global
    // This class is in use. 
    public class ReaderModuleProxy : IModule
    {
        #region IModule Members

        private LogReaderControl LogReader { get { return m_TabbedControl.LogReader; } }

        private TabbedLogReaderControl m_TabbedControl;

        private ToolStripItem[] m_MenuItems;

        public int Priority
        {
            get { return 10; }
        }

        public string DisplayName
        {
            get { return "Tab Reader"; }
        }

        public void ModuleLoaded()
        {
            //check if add an item
            if (m_TabbedControl.TabCount == 0)
            {
                m_TabbedControl.OnAdd(null, null);
                m_TabbedControl.SelectedIndex = 0;
                m_TabbedControl.OnTabSelected(null, null);
            }
        }

        public bool SupportsSendToCalls
        {
            get { return true; }
        }

        public Control ModuleControl
        {
            get { return m_TabbedControl; }
        }

        public Data.IItemDatabase Database
        {
            get { return LogReader.Database; }
            set
            {
                m_TabbedControl.Database = value;
            }
        }

        public bool CanPrint
        {
            get { return true; }
        }

        public void Print(System.Drawing.Graphics g, System.Drawing.RectangleF rect)
        {
            LogReader.Print(g, rect);
        }

        public void CustomPrint()
        {

        }

        public bool TreeVisible
        {
            get { return true; }
        }

        public string SelectedItem
        {
            get { return LogReader.SelectedItem; }
            set { LogReader.SelectedItem = value; }
        }

        public BindingListEx<string> CheckedItems
        {
            get { return LogReader.CheckedItems; }
        }

        public void Initialize(IModuleManager manager)
        {
            m_TabbedControl = new TabbedLogReaderControl();
            m_TabbedControl.PropertyChanged += m_TabbedControl_PropertyChanged;
            ToolStripItem addPage = new ToolStripButton("", Resources.Add_new_page_in_graph_module, m_TabbedControl.OnAdd);
            ToolStripItem removePage = new ToolStripButton("", Resources.Remove_current_page_from_graph_module, m_TabbedControl.OnRemove);

            m_MenuItems = new[] { addPage, removePage };

            m_TabbedControl.Initialize();
        }

        void m_TabbedControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        public void LoadSettings(SettingsGraph settings, string prefix, bool readOnly)
        {
            if (settings.TabReader == null || settings.TabReader.Tabs.Count == 0)
            {
                return;
            }

            foreach (Tab tab in settings.TabReader.Tabs)
            {
                TabPage tabPage = m_TabbedControl.AddPage();
                var lrc = (LogReaderControl)tabPage.Controls[0];
                lrc.IsTheme = readOnly;

                lrc.CheckedItems.RaiseListChangedEvents = false;
                foreach (string id in tab.IDs)
                    lrc.CheckedItems.Add(id);

                lrc.CheckedItems.RaiseListChangedEvents = true;
                lrc.CheckedItems.ResetBindings();
            }
            m_TabbedControl.SelectedIndex = 0;
            m_TabbedControl.OnTabSelected(null, null);
        }

        public void SaveSettings(SettingsGraph settings)
        {
            settings.TabReader = new TabReaderSettings { Tabs = new List<Tab>() };
            settings.TabReader.Tabs.AddRange(from TabPage tabPage in m_TabbedControl.TabPages let logReader = tabPage.Controls[0] as LogReaderControl where !logReader.IsTheme select new Tab { IDs = logReader.CheckedItems.ToList(), Name = tabPage.Text });
        }

        public void Clear()
        {

        }

        public bool HasSettingsChanged
        {
            get { return false; }
        }

        public void Shown(out ToolStripItem[] menuItems)
        {
            menuItems = m_MenuItems;
        }

        public void Hidden()
        {

        }

        public void GotoTime(string itemId, DateTime timeStamp)
        {
            m_TabbedControl.GotoTime(itemId, timeStamp);
        }

        #endregion

        #region INotifyPropertyChanged Members

        //TODO: Implement LogReaderControl
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
