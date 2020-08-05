using LogStudio.Data;
using LogStudio.Framework;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
namespace LogStudio
{
    public partial class LogGraphControl : UserControl, IModule
    {
        private IModuleManager m_ModuleManager;
        private ToolStripTextBox m_PageName;
        private ToolStripButton m_AddPageButton;
        private ToolStripButton m_AddPaneButton;
        private ToolStripButton m_RemovePageButton;
        private ToolStripButton m_RemovePaneButton;

        public LogGraphControl()
        {
            InitializeComponent();
            m_AddPageButton = new ToolStripButton("", LogStudio.Graph.Properties.Resources.AddPage, new EventHandler(m_MenuAddTab_Click));
            m_AddPageButton.ImageScaling = ToolStripItemImageScaling.None;
            m_AddPageButton.ToolTipText = LogStudio.Graph.Properties.Resources.RES_ToolTipAddPage;

            m_RemovePageButton = new ToolStripButton("", LogStudio.Graph.Properties.Resources.RemovePage, new EventHandler(m_MenuRemovePage_Click));
            m_RemovePageButton.ImageScaling = ToolStripItemImageScaling.None;
            m_RemovePageButton.ToolTipText = LogStudio.Graph.Properties.Resources.RES_ToolTipRemovePage;

            m_AddPaneButton = new ToolStripButton("", LogStudio.Graph.Properties.Resources.AddGraph, new EventHandler(m_MenuAddPane_Click));
            m_AddPaneButton.ImageScaling = ToolStripItemImageScaling.None;
            m_AddPaneButton.ToolTipText = LogStudio.Graph.Properties.Resources.RES_ToolTipAddGraph;


            m_RemovePaneButton = new ToolStripButton("", LogStudio.Graph.Properties.Resources.RemoveGraph, new EventHandler(m_MenuRemovePane_Click));
            m_RemovePaneButton.ImageScaling = ToolStripItemImageScaling.None;
            m_RemovePaneButton.ToolTipText = LogStudio.Graph.Properties.Resources.RES_ToolTipRemoveGraph;

            m_PageName = new ToolStripTextBox();
            m_PageName.Leave += new EventHandler(OnPageNameLeave);
            m_PageName.KeyDown += new KeyEventHandler(OnPageNameKeyDown);
            m_AddPaneButton.Enabled = false;
            m_RemovePageButton.Enabled = false;
            m_RemovePaneButton.Enabled = false;

            m_TabControl.SelectedIndex = -1;

            LogGraphPage page = AddPage(GenerateName("Page"), false, false, true);

            m_TabControl.VisibleChanged += (s, e) =>
            {
                if (m_TabControl.Visible && m_TabControl.SelectedTab is LogGraphPage pageTab)
                {
                    if (pageTab.ItemDatabase != m_Database)
                        pageTab.ItemDatabase = m_Database;
                }
            };

            m_TabControl.Selected += (s, e) =>
            {
                if (m_TabControl.Visible && m_TabControl.SelectedTab is LogGraphPage pageTab)
                {
                    if (pageTab.ItemDatabase != m_Database)
                        pageTab.ItemDatabase = m_Database;
                }
            };
        }

        void OnPageNameKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                SelectedPage.Text = m_PageName.Text;
            }
        }


        void OnPageNameLeave(object sender, EventArgs e)
        {
            SelectedPage.Text = m_PageName.Text;
        }



        private bool m_HasChanged = false;

        public bool GetHasChanged()
        {
            bool changed = m_HasChanged;

            if (changed)
                return changed;

            foreach (LogGraphPage page in m_TabControl.TabPages)
            {
                if (page.GetHasChanged())
                {
                    changed = true;
                    break;
                }
            }

            return changed;
        }


        private IItemDatabase m_Database = null;

        public LogGraphPage AddPage(string title, bool markAsChanged, bool readOnly, bool addPane)
        {
            LogGraphPage tab = new LogGraphPage(m_ModuleManager, readOnly, addPane);

            if (string.IsNullOrEmpty(title))
                tab.Text = GenerateName("Page");
            else
                tab.Text = title;

            tab.ItemDatabase = m_Database;

            m_TabControl.TabPages.Add(tab);

            m_TabControl.SelectedTab = tab;

            if (markAsChanged)
                m_HasChanged = true;

            tab.OnSelectedPaneChanged += new EventHandler(OnSelectedPaneChanged);
            tab.OnSelectedCurveItemChanged += new EventHandler(OnSelectedCurveItemChanged);
            return tab;
        }

        void OnSelectedPaneChanged(object sender, EventArgs e)
        {
            UpdateButtonStatus();
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("CheckedItems"));

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedItem"));
        }

        void OnSelectedCurveItemChanged(object sender, EventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedItem"));
        }

        public string GenerateName(string title)
        {
            int count = 1;
            string name;
            do
            {
                name = string.Format("[New {1} {0}]", title, count);
                count++;
            }
            while (NameAlreadyExists(name));

            return name;
        }

        public bool NameAlreadyExists(string name)
        {
            return m_TabControl.TabPages.Cast<LogGraphPage>().Any(tab => tab.Text == name);
        }

        public void RenameCurrentPage(string name)
        {
            if (m_TabControl.SelectedIndex >= 0)
                m_TabControl.SelectedTab.Text = name;

            m_HasChanged = true;
        }

        public void RemoveCurrentPage()
        {
            RemovePage(SelectedPage);
        }

        public void RemovePage(LogGraphPage page)
        {
            if (page != null)
            {
                page.ClearAllGraphs();

                page.OnSelectedPaneChanged -= new EventHandler(OnSelectedPaneChanged);
                page.OnSelectedCurveItemChanged -= new EventHandler(OnSelectedCurveItemChanged);

                m_TabControl.TabPages.Remove(page);

                m_HasChanged = true;
            }
        }

        public LogGraphPage SelectedPage
        {
            get
            {
                return (LogGraphPage)m_TabControl.SelectedTab;
            }
            set
            {
                if (value != null)
                    m_TabControl.SelectedTab = value;
            }
        }

        private void m_TabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            //    if (SelectedPage != null)
            //        SelectedPage.SelectedPane = null;
        }

        private void m_MenuAddTab_Click(object sender, EventArgs e)
        {
            AddPage(string.Empty, true, false, true);
        }

        private void m_MenuRemovePage_Click(object sender, EventArgs e)
        {
            RemovePage(SelectedPage);
        }

        private void m_MenuRemovePane_Click(object sender, EventArgs e)
        {
            if (SelectedPage != null)
                SelectedPage.RemovePane(SelectedPage.SelectedPane);
        }

        private void m_MenuAddPane_Click(object sender, EventArgs e)
        {
            if (SelectedPage != null)
                SelectedPage.AddPane(true);
        }


        public void Print(Graphics g, RectangleF rect)
        {
            SelectedPage.Print(g, rect);
        }

        public void ModuleLoaded()
        {

        }

        public void Clear()
        {
            foreach (LogGraphPage tab in m_TabControl.TabPages)
            {
                tab.ClearAllGraphs();
            }

            m_TabControl.TabPages.Clear();

            m_HasChanged = true;
        }

        public void LoadSettings(SettingsGraph settings, string prefix, bool readOnly)
        {
            if (settings.Pages.Count > 0)
            {
                m_TabControl.TabPages.Clear();
            }
            foreach (PageSettings page in settings.Pages)
            {
                AddPage(prefix + page.Name, true, readOnly, false).LoadSettings(page);
            }

            if (m_TabControl.TabCount > 0)
                m_TabControl.SelectedIndex = 0;

            if (m_TabControl.SelectedTab != null)
            {
                ((LogGraphPage)m_TabControl.SelectedTab).SelectFirstPane();
            }

            m_HasChanged = false;
        }

        public void SaveSettings(SettingsGraph settings)
        {
            foreach (LogGraphPage page in m_TabControl.TabPages)
            {
                PageSettings pageSettings = new PageSettings();
                pageSettings.Name = page.Text;

                page.SaveSettings(pageSettings);
                settings.Pages.Add(pageSettings);
            }
            m_HasChanged = false;
        }

        #region IModule Members

        public int Priority
        {
            get { return 100; }
        }

        public string DisplayName
        {
            get { return "Graph"; }
        }

        public Image Icon
        {
            get { return null; }
        }

        public Control ModuleControl
        {
            get { return this; }
        }

        public IItemDatabase Database
        {
            get
            {
                return m_Database;
            }
            set
            {
                foreach (LogGraphPage tab in m_TabControl.TabPages)
                {
                    tab.ItemDatabase = null;
                }

                m_Database = value;

                if (m_TabControl.Visible && m_TabControl.SelectedTab is LogGraphPage pageTab)
                {
                    if (pageTab.ItemDatabase != m_Database)
                        pageTab.ItemDatabase = m_Database;
                }
            }
        }



        public bool CanPrint
        {
            get { return SelectedPage != null; }
        }

        public bool TreeVisible
        {
            get { return true; }
        }

        public string SelectedItem
        {
            get
            {
                if (SelectedPage != null && SelectedPage.SelectedPane != null)
                    return SelectedPage.SelectedPane.SelectedItem;
                else
                    return string.Empty;
            }
            set
            {
                if (SelectedPage != null && SelectedPage.SelectedPane != null)
                    SelectedPage.SelectedPane.SelectedItem = value;
            }
        }

        public BindingListEx<string> CheckedItems
        {
            get
            {
                if (SelectedPage != null && SelectedPage.SelectedPane != null)
                    return SelectedPage.SelectedPane.CheckedItems;
                else
                    return null;
            }
        }

        public void Initialize(IModuleManager manager)
        {
            m_ModuleManager = manager;
        }

        public bool HasSettingsChanged
        {
            get { return GetHasChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        private void m_TabControl_Selected(object sender, TabControlEventArgs e)
        {
            UpdateButtonStatus();

            if (e.TabPage != null)
                ((LogGraphPage)e.TabPage).Selected();

            if (e.TabPage != null)
                OnSelectedPaneChanged(sender, EventArgs.Empty);

            //           if (PropertyChanged != null)
            //               PropertyChanged(sender, new PropertyChangedEventArgs("CheckedItems"));


        }

        private void UpdateButtonStatus()
        {
            m_AddPaneButton.Enabled = SelectedPage != null;
            m_RemovePageButton.Enabled = SelectedPage != null;
            m_RemovePaneButton.Enabled = SelectedPage != null && SelectedPage.SelectedPane != null;
            m_PageName.Enabled = SelectedPage != null && !SelectedPage.ReadOnly;

            if (SelectedPage != null)
                m_PageName.Text = SelectedPage.Text;
            else
                m_PageName.Text = "";
        }



        public void Shown(out ToolStripItem[] menuItems)
        {
            menuItems = new ToolStripItem[]
            {
                m_PageName,
                m_AddPageButton,
                m_RemovePageButton,
                new ToolStripSeparator(),
                m_AddPaneButton,
                m_RemovePaneButton
            };
        }

        public void Hidden()
        {
        }

        #endregion

        #region IModule Members

        public bool SupportsSendToCalls
        {
            get { return false; }
        }

        public void GotoTime(string itemId, DateTime timeStamp)
        {

        }

        #endregion
    }

    public class LogGraphTabEventArgs : EventArgs
    {
        public LogGraphTabEventArgs(LogGraphPage tab)
        {
            Tab = tab;
        }

        public LogGraphPage Tab { get; private set; }
    }
}
