using JWC;
using log4net;
using LogStudio.Data;
using LogStudio.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LogStudio
{
    public partial class Form1 : Form
    {
        private LogDatabase m_Database;
        private SettingsGraph m_Settings = new SettingsGraph();
        private FilterSettings m_AllowedItemSettings = new FilterSettings();
        private bool m_AdminMode;

        ModuleManager m_ModuleManager;

        public Form1()
        {
            InitializeComponent();
        }

        private void OnCloseClick(object sender, EventArgs e)
        {
            Close();
        }

        private void OnOpenClick(object sender, EventArgs e)
        {
            try
            {

                if (m_OpenDlg.ShowDialog() == DialogResult.OK)
                {
                    OpenLogFile(m_OpenDlg.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName);
            }
        }

        private void UpdateControlsState()
        {
            m_FileMenuStrip.SuspendLayout();

            m_PrintButton.Enabled = m_ModuleManager.SelectedModule != null && m_ModuleManager.SelectedModule.CanPrint;
            m_PrintPreviewMenu.Enabled = m_PrintButton.Enabled;
            m_PrintPreview.Enabled = m_PrintButton.Enabled;
            m_PrintMenu.Enabled = m_PrintButton.Enabled;
            m_Close.Enabled = m_Database != null;
            m_FileMenuStrip.ResumeLayout(true);
            beamCurrentCompensationToolStripMenuItem.Enabled = m_Database != null;
            compareLogFileToolStripMenuItem.Enabled = m_Database != null;
        }

        private void CloseFile()
        {
            if (m_Database != null)
            {
                m_ItemTree.ItemDatabase = null;

                foreach (IModule module in m_ModuleManager.Modules)
                {
                    module.Database = null;
                }

                m_Database.MultipleLogFilesQuery -= OnMultipleLogFileQuery;
                m_Database.OnParserReadError -= OnParserReadErrorCallback;
                m_Database.Dispose();
                m_Database = null;
            }
        }

        private void OpenLogFile(string filename)
        {
            CloseFile();


            if (CheckIfNetworkFile(filename))
            {
                if (MessageBox.Show(this, "It seems like the log file is uncompressed and lives on a network device.\r\nThis might degrade your Log Studio experience.\r\nAre you sure you want to continue?", "Performance issue...", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }
            }

            m_ModuleTabControl.Enabled = false;
            
            m_Database = new LogDatabase(filename, true, m_AdminMode ? null : m_AllowedItemSettings.AllowedItems);
            m_Database.OnParserReadError += OnParserReadErrorCallback;
            m_Database.MultipleLogFilesQuery += OnMultipleLogFileQuery;
            

            m_ItemTree.ItemDatabase = m_Database;
            foreach (IModule module in m_ModuleManager.Modules)
            {
                module.Database = m_Database;
            }

            m_Database.OnInitialReadDone += OnDatabaseOnInitialReadDone; 
            m_Database.Start();

            UpdateTitle();
            m_MruMenu.AddFile(filename);
            UpdateControlsState();
        }

        private void OnDatabaseOnInitialReadDone(object sender, EventArgs args)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<EventArgs>(OnDatabaseOnInitialReadDone), sender, args);
                return;
            }
            
            m_ModuleTabControl.Enabled = true;
        }

        private static bool CheckIfNetworkFile(string filename)
        {
            var uri = new Uri(filename);

            if (uri.IsUnc)
                return true;

            var dirInfo = new DirectoryInfo(filename);
            if (dirInfo.Root != null)
            {
                var di = new DriveInfo(dirInfo.Root.Name);
                return di.DriveType == DriveType.Network;
            }

            return false;
        }

        static void OnMultipleLogFileQuery(object sender, MultipplePLGFilesArgs e)
        {
            using (var win = new MultippleLogfilesSelect())
            {
                if (win.ShowDialog(e.AvailableFilenames) == DialogResult.OK)
                {
                    e.SelectedFilename = win.Filename;
                }
                else
                    e.Cancel = true;
            }
        }


        private void OnParserReadErrorCallback(object sender, ExceptionEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<ExceptionEventArgs>(OnParserReadErrorCallback), sender, e);
                return;
            }

            MessageBox.Show(e.Exception.Message, Properties.Resources.RES_FailedToReadFile);
        }

        private void UpdateTitle()
        {
            var sb = new StringBuilder();

            sb.Append(Application.ProductName);

            if (IntPtr.Size == 4)
                sb.Append(" (32-Bit)");
            else if (IntPtr.Size == 8)
                sb.Append(" (64-Bit)");

#if BETA
            sb.Append(" BETA");
#endif

            if (m_AdminMode)
                sb.Append(" [Admin Mode] ");

            if (m_Database != null)
                sb.Append(" - " + Path.GetFileName(m_Database.Filename));

            Text = sb.ToString();
        }

        private MruStripMenu m_MruMenu;
        private readonly string m_MruRegisterKey = @"Software\LogStudio";

        private void OnMruOpenFile(int number, string filename)
        {
            if (File.Exists(filename))
                OpenLogFile(filename);
            else
            {
                m_MruMenu.RemoveFile(number);
                MessageBox.Show($"File {filename} no longer exist.");
            }
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            //Recent file history
            m_MruMenu = new MruStripMenuInline(fileToolStripMenuItem, m_RecentFileHistory, OnMruOpenFile, m_MruRegisterKey + "\\MRU", 8);

            m_AdminMode = Environment.CommandLine.ToLower().Contains("-admin");

            m_ModuleManager = new ModuleManager(LogManager.GetLogger("ApplicationLog"), m_AdminMode, m_FileMenuStrip);
            m_ModuleManager.PropertyChanged += OnModuleManagerPropertyChanged;
            m_ModuleManager.OnModulePropertyChanged += OnModulePropertyChanged;
            m_ModuleManager.LoadPlugins();

            foreach (IModule module in m_ModuleManager.Modules)
            {
                if (module.ModuleControl == null)
                    continue;

                var page = new TabPage(module.DisplayName);
                module.ModuleControl.Dock = DockStyle.Fill;
                page.Controls.Add(module.ModuleControl);
                page.Name = module.DisplayName;
                page.Tag = module;
                m_ModuleTabControl.TabPages.Add(page);
            }

            OnTabSelected(this, new TabControlEventArgs(null, -1, TabControlAction.Selected));
            m_Task.Text = string.Empty;

            if (File.Exists(Path.ChangeExtension(Application.ExecutablePath, ".ftr")))
                m_AllowedItemSettings = FilterSettings.FromFile(Path.ChangeExtension(Application.ExecutablePath, ".ftr"));

            try
            {
                if (File.Exists(Path.ChangeExtension(Application.ExecutablePath, ".data")))
                    m_Settings = SettingsGraph.FromFile(Path.ChangeExtension(Application.ExecutablePath, ".data"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Failed to load settings file!");
            }

            var userSettings = new SettingsGraph();

            try
            {
                if (File.Exists(Properties.Settings.Default.SettingsFile))
                    userSettings = SettingsGraph.FromFile(Properties.Settings.Default.SettingsFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Failed to load User settings file!");
            }

            string[] args = Environment.GetCommandLineArgs();

            for (int i = 1; i < args.Length; i++)
            {
                string arg = args[i];
                switch (arg)
                {
                    default:
                        if (File.Exists(arg) && m_Database == null)
                            OpenLogFile(arg);
                        break;
                }
            }

            foreach (IModule module in m_ModuleManager.Modules)
            {
                if (Properties.Settings.Default.LoadSettings)
                    module.LoadSettings(m_Settings, Properties.Resources.RES_PagePrefix, true);

                module.LoadSettings(userSettings, string.Empty, false);

                module.ModuleLoaded();
            }


            m_Admin.Visible = m_AdminMode;

            UpdateTitle();
        }


        void OnModulePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateControlsState();
        }

        void OnModuleManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            m_ItemTree.Module = m_ModuleManager.SelectedModule;
            //splitContainer1.Panel1Collapsed = !(m_ModuleManager.SelectedModule != null && m_ModuleManager.SelectedModule.TreeVisible);
            m_ModuleLabel.Text = m_ModuleManager.SelectedModule.DisplayName;
            UpdateControlsState();

            if (e.PropertyName == "SelectedModule")
            {
                TabPage page = m_ModuleTabControl.TabPages[m_ModuleManager.SelectedModule.DisplayName];

                if (m_ModuleTabControl.SelectedTab != page)
                {
                    m_ModuleTabControl.SelectedTab = page;
                }
            }
            //if (m_ModuleManager.SelectedModule != null)
            //    splitContainer1.Panel1Collapsed = !m_ModuleManager.SelectedModule.TreeVisible;
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            m_ModifiedTimer.Enabled = false;
            m_Database?.Dispose();
            m_MruMenu.SaveToRegistry();
        }

        private void OnTabSelected(object sender, TabControlEventArgs e)
        {
            UpdateControlsState();

            if (e.TabPage != null)
                m_ModuleManager.SelectedModule = e.TabPage.Tag as IModule;
        }



        private void OnPrintDocumentPrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            if (m_ModuleManager.SelectedModule != null && m_ModuleManager.SelectedModule.CanPrint)
                m_ModuleManager.SelectedModule.Print(e.Graphics, e.MarginBounds);
        }

        private void OnPrintPreviewClick(object sender, EventArgs e)
        {
            m_DlgPrintPreview.ShowDialog();
        }

        private void OnPrintButtonClick(object sender, EventArgs e)
        {
            if (m_DlgPrint.ShowDialog() == DialogResult.OK)
                m_PrintDocument.Print();
        }

        private void OnSettingsNewMenu(object sender, EventArgs e)
        {
            bool hasSettingsChanged = false;

            foreach (IModule module in m_ModuleManager.Modules)
            {
                hasSettingsChanged |= module.HasSettingsChanged;
            }

            if (hasSettingsChanged)
            {
                if (MessageBox.Show(Properties.Resources.RES_LooseChanges, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    return;
            }

            Properties.Settings.Default.SettingsFile = string.Empty;
            Properties.Settings.Default.Save();

            foreach (IModule module in m_ModuleManager.Modules)
            {
                module.Clear();
                module.LoadSettings(m_Settings, Properties.Resources.RES_PagePrefix, true);
            }
        }

        private void OnSettingsOpenMenu(object sender, EventArgs e)
        {
            bool hasSettingsChanged = false;

            foreach (IModule module in m_ModuleManager.Modules)
            {
                hasSettingsChanged |= module.HasSettingsChanged;
            }

            if (hasSettingsChanged)
            {
                if (MessageBox.Show(Properties.Resources.RES_LooseChanges, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    return;
            }

            m_DlgOpenSettings.InitialDirectory = GetSaveFolder();

            if (m_DlgOpenSettings.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.SettingsFile = string.Empty;
                Properties.Settings.Default.Save();

                SettingsGraph userSettings = SettingsGraph.FromFile(m_DlgOpenSettings.FileName);

                foreach (IModule module in m_ModuleManager.Modules)
                {
                    module.Clear();
                    module.LoadSettings(m_Settings, Properties.Resources.RES_PagePrefix, true);
                    module.LoadSettings(userSettings, string.Empty, false);
                }

                Properties.Settings.Default.SettingsFile = m_DlgOpenSettings.FileName;
                Properties.Settings.Default.Save();
            }
        }

        private void OnSettingsSaveMenu(object sender, EventArgs e)
        {
            SettingsSave();
        }

        private void SettingsSave()
        {
            bool fileIsOk = false;

            if (File.Exists(Properties.Settings.Default.SettingsFile))
            {
                fileIsOk = (File.GetAttributes(Properties.Settings.Default.SettingsFile) & FileAttributes.ReadOnly) != FileAttributes.ReadOnly;
            }

            if (fileIsOk)
            {
                var graph = new SettingsGraph();

                foreach (IModule module in m_ModuleManager.Modules)
                {
                    module.SaveSettings(graph);
                }

                try
                {
                    graph.ToFile(Properties.Settings.Default.SettingsFile);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, @"Failed to save user settings file!");
                }
            }
            else
            {
                SettingsSaveAs();
            }
        }

        private void OnSettingsSaveAsMenu(object sender, EventArgs e)
        {
            SettingsSaveAs();
        }

        private static string GetSaveFolder()
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.SettingsFile))
                return Properties.Settings.Default.SettingsFile;
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Log Studio");
        }

        private void SettingsSaveAs()
        {
            m_DlgSaveSettings.InitialDirectory = GetSaveFolder();

            if (m_DlgSaveSettings.ShowDialog() == DialogResult.OK)
            {
                var graph = new SettingsGraph();

                foreach (IModule module in m_ModuleManager.Modules)
                {
                    module.SaveSettings(graph);
                }

                try
                {
                    graph.ToFile(m_DlgSaveSettings.FileName);
                    Properties.Settings.Default.SettingsFile = m_DlgSaveSettings.FileName;
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, @"Failed to save user settings file!");
                }
            }
        }

        private void OnPageSetupClick(object sender, EventArgs e)
        {
            m_DlgPageSetup.ShowDialog();
        }

        private void OnModifiedTimerTick(object sender, EventArgs e)
        {
            bool hasSettingsChanged = false;

            foreach (IModule module in m_ModuleManager.Modules)
            {
                hasSettingsChanged |= module.HasSettingsChanged;
            }

            if (hasSettingsChanged)
            {
                if (m_Modified.Text != Properties.Resources.RES_Modified)
                    m_Modified.Text = Properties.Resources.RES_Modified;
            }
            else
            {
                if (m_Modified.Text != string.Empty)
                    m_Modified.Text = string.Empty;
            }

            if (m_Database != null)
            {
                switch (m_Database.State)
                {
                    case ProgressStateEnum.Starting:
                        m_Task.Text = @"Parsing log file...";
                        break;
                    case ProgressStateEnum.InProgress:
                        m_StatusStrip.SuspendLayout();
                        m_Progress.Value = m_Database.Progress;
                        m_Task.Text =
                            $"Parsing log file {m_Database.BytesPerSecond / (1024 * 1024):N2} MB/s, elapsed {m_Database.ElapsedTime} s.";
                        m_StatusStrip.ResumeLayout();
                        break;
                    case ProgressStateEnum.Monitoring:
                        m_StatusStrip.SuspendLayout();
                        m_Progress.Value = 0;
                        m_Task.Text = $"Monitoring file {Path.GetFileName(m_Database.Filename)} for changes.";
                        m_StatusStrip.ResumeLayout();
                        break;
                    case ProgressStateEnum.Done:
                        m_Progress.Value = 0;
                        m_Task.Text = $"Parsed in {m_Database.ElapsedTime} s.";
                        break;
                    case ProgressStateEnum.Decompressing:
                        m_Progress.Value = m_Database.Progress;
                        m_Task.Text =
                            $"Decompressing log file {m_Database.BytesPerSecond / (1024 * 1024):N2} MB/s, elapsed {m_Database.ElapsedTime} s.";
                        break;
                }
            }
            else
            {
                m_StatusStrip.SuspendLayout();
                m_Progress.Value = 0;
                m_Task.Text = string.Empty;
                m_StatusStrip.ResumeLayout();
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.FormOwnerClosing)
            {
                bool hasSettingsChanged = false;

                foreach (IModule module in m_ModuleManager.Modules)
                {
                    hasSettingsChanged |= module.HasSettingsChanged;
                }

                if (hasSettingsChanged)
                {
                    DialogResult res = MessageBox.Show(
                        Properties.Resources.RES_SaveChanges,
                        Application.ProductName,
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button3
                        );

                    if (res == DialogResult.Yes)
                        SettingsSave();
                    else if (res == DialogResult.Cancel)
                        e.Cancel = true;
                }
            }
        }

        private void OnEditFilterClicked(object sender, EventArgs e)
        {
            using (var form = new ItemSelectionForm())
            {
                if (form.ShowDialog(m_Database, m_AllowedItemSettings.AllowedItems.ToArray()) == DialogResult.OK)
                {
                    m_AllowedItemSettings.AllowedItems.Clear();

                    foreach (string item in form.SelectedItems)
                    {
                        m_AllowedItemSettings.AllowedItems.Add(item);
                    }

                    m_AllowedItemSettings.ToFile(Path.ChangeExtension(Application.ExecutablePath, ".ftr"));

                    MessageBox.Show(Properties.Resources.RES_ChangesNeedsRestart, Application.ProductName);
                }
            }
        }

        private void OnAdminOpening(object sender, EventArgs e)
        {
            m_FilterEditor.Enabled = m_Database != null;
        }

        private void settingsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            bool fileIsOk = false;

            if (File.Exists(Properties.Settings.Default.SettingsFile))
            {
                fileIsOk = (File.GetAttributes(Properties.Settings.Default.SettingsFile) & FileAttributes.ReadOnly) != FileAttributes.ReadOnly;
            }

            m_SettingsSave.Enabled = fileIsOk;
        }

        private void m_Statistics_Click(object sender, EventArgs e)
        {
            if (m_Database == null)
                return;

            using (var win = new Statistics())
            {
                win.Initialize(m_Database);
                win.ShowDialog();
            }
        }

        private void reportDesignerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                const string filename = "UserManual.chm";

                using (Process p = Process.Start(filename))
                {
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName);
            }

        }

        private void OnShowAbout(object sender, EventArgs e)
        {
            using (var win = new AboutBox())
            {
                win.ShowDialog();
            }
        }

        private void m_ToolsMenu_DropDownOpening(object sender, EventArgs e)
        {
            m_Statistics.Enabled = m_Database != null;
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void OnDragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (!CanFocus)
                    return;
                var a = (Array)e.Data.GetData(DataFormats.FileDrop);

                if (a != null)
                {
                    // Extract string from first array element
                    // (ignore all files except first if number of files are dropped).
                    string filename = a.GetValue(0).ToString();

                    OpenLogFile(filename);
                    Activate();        // in the case Explorer overlaps this form
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error in DragDrop function: " + ex.Message);
                // don't show MessageBox here - Explorer is waiting !
            }
        }

        private void m_Close_Click(object sender, EventArgs e)
        {
            CloseFile();
            UpdateControlsState();
            UpdateTitle();
        }

        private void BeamCurrentCompensationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_Database != null)
            {
                var beamCurrentCompensationForm = new BeamCurrentCompensationForm(m_Database);
                beamCurrentCompensationForm.ShowDialog();
            }
        }
        public string ApproximatedTimeData()
        {
            try
            {
                string estBuildDone = "Process.TimeApproximation.BuildDoneTime";
                string buildDone = "Alarms.BuildDone";
                //string estimatedTotalTime = "Process.TimeApproximation.EstimatedBuildDoneTime";
                string heatStartPlateDone = "Process.ProcessManager.HeatStartPlateDone";
                //var allBuildDone = new FullnameEnumerator(currents, m_Database).ToArray();
                //var allFeedback = new FullnameEnumerator(feedback, m_Database).ToArray();

                LogRowData buildDoneItem = m_Database.GetLastItem(buildDone, p => p.Value == "Set");
                string output = string.Empty;
                if (buildDoneItem != null)
                {
                    DateTime buildDoneTime = buildDoneItem.TimeStamp;
                    LogRowData buildStartMeltTime = m_Database.GetLastItem(heatStartPlateDone);
                    IEnumerable<LogRowData> allEstBuildDoneItems = m_Database.GetAll(estBuildDone).Where(p =>
                        p.TimeStamp >= buildStartMeltTime.TimeStamp &&
                        p.TimeStamp < buildDoneTime - TimeSpan.FromMinutes(15));

                    output = $"Build done: \t\t{buildDoneTime}\r\n\r\n";
                    foreach (LogRowData estBuildDoneItem in allEstBuildDoneItems)
                    {
                        DateTime estDoneTime = DateTime.Parse(estBuildDoneItem.Value);
                        TimeSpan totalTimeDiff = estDoneTime - buildDoneTime;
                        TimeSpan timeForEstimation = buildDoneTime - estBuildDoneItem.TimeStamp;
                        double diff = Math.Abs(totalTimeDiff.TotalSeconds / timeForEstimation.TotalSeconds);
                        output +=
                            $"{estBuildDoneItem.TimeStamp}:\t{estDoneTime} \t{totalTimeDiff:d\\.hh\\:mm} \t{100 * diff:0.0}%\r\n";
                    }
                }

                return output;
            }
            catch (Exception)
            {
            }

            return string.Empty;
        }

        private void CompareLogFile(object o, EventArgs args)
        {
            var textWindow = new Form
            {
                Text = "Compare themes",
                Width = 600,
                Height = 600
            };
            var txt = new RichTextBox
            {
                ShortcutsEnabled = true,
                Text = "Loading second file...",
                Width = 580,
                Height = 570,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                ReadOnly = true,
                Multiline = true
            };
            textWindow.Controls.Add(txt);
            textWindow.Show();
            LogDatabase referenceDb = null;
            try
            {
                var openReferenceLogFileDialog = new OpenFileDialog
                {
                    Filter = "Log files |*.zip; *.plg|All files (*.*)|*.*",
                    FilterIndex = 1,
                    Multiselect = false,
                    Title = "Select reference file"
                };

                DialogResult userClickedOK = openReferenceLogFileDialog.ShowDialog();
                if (userClickedOK != DialogResult.OK)
                    return;

                referenceDb = new LogDatabase(openReferenceLogFileDialog.FileName, true, null);
                referenceDb.Start();
                //wait while indexing reference log file database
                while (referenceDb.State != ProgressStateEnum.Done && referenceDb.State != ProgressStateEnum.Monitoring)
                {
                    Thread.Sleep(500);
                }

                //Add ids in themes and machine, except process themes
                Func<string[], Dictionary<string, string>> getIds = ids =>
                    ids.Where(p => p.StartsWith("Machine.")).Concat(ids.Where(p => p.StartsWith("Themes.")
                                                                                   && !p.StartsWith(
                                                                                       "Themes.ProcessStep[")))
                        .ToDictionary(p => p);

                Dictionary<string, string> loadedIds = getIds(m_Database.GetItemsIDs().ToArray());
                Dictionary<string, string> referenceIds = getIds(referenceDb.GetItemsIDs().ToArray());

                KeyValuePair<string, string>[] changesInLoadedIds =
                    loadedIds.Where(p => m_Database.GetAll(p.Key).Count() > 1).ToArray();
                KeyValuePair<string, string>[] changesInReferenceIds =
                    referenceIds.Where(p => referenceDb.GetAll(p.Key).Count() > 1).ToArray();

                string output = $"Comparing {m_Database.Filename} with reference {referenceDb.Filename}" +
                                Environment.NewLine + Environment.NewLine;
                if (changesInLoadedIds.Length > 0)
                    output += "These items have multiple entries in the log file. Using the last entry as comparison:" +
                              Environment.NewLine;
                output = changesInLoadedIds.Aggregate(output,
                    (current, changesInLoadedId) => current + changesInLoadedId.Key + Environment.NewLine);
                if (changesInReferenceIds.Length > 0)
                    output += Environment.NewLine +
                              "These items have multiple entries in the reference file. Using the last entry as comparison:" +
                              Environment.NewLine;
                output = changesInReferenceIds.Aggregate(output,
                    (current, changesInRefId) => current + changesInRefId.Key + Environment.NewLine);
                output += Environment.NewLine;

                output += "Changes compared to reference file:" + Environment.NewLine;
                foreach (KeyValuePair<string, string> id in referenceIds)
                {
                    bool compareExist = m_Database.Exists(id.Key);
                    string refValue = referenceDb.GetLastItem(id.Key, t => t.CycleIndex >= 0).Value;
                    if (compareExist)
                    {
                        string compareValue = m_Database.GetLastItem(id.Key, t => t.CycleIndex >= 0).Value;
                        if (compareValue != refValue)
                        {
                            output += $"Change \t{id.Value} \t{refValue} \t->\t {compareValue}" + Environment.NewLine;
                        }
                    }
                    else
                    {
                        output += $"Removed \t{id.Value} \t{refValue}" + Environment.NewLine;
                    }
                }

                foreach (KeyValuePair<string, string> loadedId in loadedIds.Where(p => !referenceDb.Exists(p.Key)))
                {
                    string compareValue = m_Database.GetLastItem(loadedId.Key, t => t.CycleIndex >= 0).Value;
                    output += $"New \t{loadedId.Value} \t{compareValue}" + Environment.NewLine;
                }
                txt.Text = output;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                referenceDb?.Dispose();
            }
        }

        private void compareLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CompareLogFile(sender, e);
        }

		private void correlationToolStripMenuItem_Click(object sender, EventArgs e)
		{
            if (m_Database == null)
				return;
            
			string selected = m_ItemTree.Module.SelectedItem;
			if (string.IsNullOrEmpty(selected))
			{
				MessageBox.Show("Select a parameter.", "No parameter selected", MessageBoxButtons.OK);
				return;
			}

			using (var win = new Correlation())
			{
				win.Initialize(m_Database, selected);
				win.ShowDialog();
			}
		}

	}
}
