namespace LogStudio
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.m_ItemTree = new LogStudio.ItemsTreeView();
			this.m_ModuleTabControl = new System.Windows.Forms.TabControl();
			this.m_TabImages = new System.Windows.Forms.ImageList(this.components);
			this.m_FileMenuStrip = new System.Windows.Forms.ToolStrip();
			this.m_OpenButton = new System.Windows.Forms.ToolStripButton();
			this.m_PrintButton = new System.Windows.Forms.ToolStripButton();
			this.m_PrintPreview = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.m_ModuleLabel = new System.Windows.Forms.ToolStripLabel();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.m_OpenMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.m_Close = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.m_PageSetup = new System.Windows.Forms.ToolStripMenuItem();
			this.m_PrintPreviewMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.m_PrintMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.m_RecentFileHistory = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.m_SettingsSave = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.m_ToolsMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.m_Statistics = new System.Windows.Forms.ToolStripMenuItem();
			this.troubleshootingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.compareLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.beamCurrentCompensationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.correlationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.m_Admin = new System.Windows.Forms.ToolStripMenuItem();
			this.m_FilterEditor = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.m_StatusStrip = new System.Windows.Forms.StatusStrip();
			this.m_Progress = new System.Windows.Forms.ToolStripProgressBar();
			this.m_Task = new System.Windows.Forms.ToolStripStatusLabel();
			this.m_Modified = new System.Windows.Forms.ToolStripStatusLabel();
			this.m_UpdatingLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.m_UpdatingProgress = new System.Windows.Forms.ToolStripProgressBar();
			this.m_DlgPrintPreview = new System.Windows.Forms.PrintPreviewDialog();
			this.m_PrintDocument = new System.Drawing.Printing.PrintDocument();
			this.m_DlgPrint = new System.Windows.Forms.PrintDialog();
			this.m_DlgPageSetup = new System.Windows.Forms.PageSetupDialog();
			this.m_ModifiedTimer = new System.Windows.Forms.Timer(this.components);
			this.m_DlgSaveSettings = new System.Windows.Forms.SaveFileDialog();
			this.m_OpenDlg = new System.Windows.Forms.OpenFileDialog();
			this.m_DlgOpenSettings = new System.Windows.Forms.OpenFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.m_FileMenuStrip.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.m_StatusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			resources.ApplyResources(this.splitContainer1, "splitContainer1");
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.m_ItemTree);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.m_ModuleTabControl);
			// 
			// m_ItemTree
			// 
			resources.ApplyResources(this.m_ItemTree, "m_ItemTree");
			this.m_ItemTree.ItemDatabase = null;
			this.m_ItemTree.Module = null;
			this.m_ItemTree.Name = "m_ItemTree";
			// 
			// m_ModuleTabControl
			// 
			resources.ApplyResources(this.m_ModuleTabControl, "m_ModuleTabControl");
			this.m_ModuleTabControl.ImageList = this.m_TabImages;
			this.m_ModuleTabControl.Multiline = true;
			this.m_ModuleTabControl.Name = "m_ModuleTabControl";
			this.m_ModuleTabControl.SelectedIndex = 0;
			this.m_ModuleTabControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.OnTabSelected);
			// 
			// m_TabImages
			// 
			this.m_TabImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("m_TabImages.ImageStream")));
			this.m_TabImages.TransparentColor = System.Drawing.Color.Transparent;
			this.m_TabImages.Images.SetKeyName(0, "graphhs.png");
			this.m_TabImages.Images.SetKeyName(1, "Control_ListBox.bmp");
			this.m_TabImages.Images.SetKeyName(2, "book_report.bmp");
			// 
			// m_FileMenuStrip
			// 
			this.m_FileMenuStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.m_FileMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.m_FileMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_OpenButton,
            this.m_PrintButton,
            this.m_PrintPreview,
            this.toolStripSeparator3,
            this.m_ModuleLabel});
			resources.ApplyResources(this.m_FileMenuStrip, "m_FileMenuStrip");
			this.m_FileMenuStrip.Name = "m_FileMenuStrip";
			// 
			// m_OpenButton
			// 
			this.m_OpenButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_OpenButton.Image = global::LogStudio.Properties.Resources.Open_folder;
			resources.ApplyResources(this.m_OpenButton, "m_OpenButton");
			this.m_OpenButton.Name = "m_OpenButton";
			this.m_OpenButton.Click += new System.EventHandler(this.OnOpenClick);
			// 
			// m_PrintButton
			// 
			this.m_PrintButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.m_PrintButton, "m_PrintButton");
			this.m_PrintButton.Image = global::LogStudio.Properties.Resources.Print;
			this.m_PrintButton.Name = "m_PrintButton";
			this.m_PrintButton.Click += new System.EventHandler(this.OnPrintButtonClick);
			// 
			// m_PrintPreview
			// 
			this.m_PrintPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.m_PrintPreview, "m_PrintPreview");
			this.m_PrintPreview.Image = global::LogStudio.Properties.Resources.Preview;
			this.m_PrintPreview.Name = "m_PrintPreview";
			this.m_PrintPreview.Click += new System.EventHandler(this.OnPrintPreviewClick);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
			// 
			// m_ModuleLabel
			// 
			this.m_ModuleLabel.Name = "m_ModuleLabel";
			resources.ApplyResources(this.m_ModuleLabel, "m_ModuleLabel");
			// 
			// menuStrip1
			// 
			this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.m_ToolsMenu,
            this.m_Admin,
            this.helpToolStripMenuItem});
			resources.ApplyResources(this.menuStrip1, "menuStrip1");
			this.menuStrip1.Name = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_OpenMenu,
            this.toolStripSeparator1,
            this.m_Close,
            this.toolStripSeparator6,
            this.m_PageSetup,
            this.m_PrintPreviewMenu,
            this.m_PrintMenu,
            this.toolStripSeparator2,
            this.m_RecentFileHistory,
            this.toolStripSeparator5,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
			// 
			// m_OpenMenu
			// 
			this.m_OpenMenu.Image = global::LogStudio.Properties.Resources.Open_folder;
			resources.ApplyResources(this.m_OpenMenu, "m_OpenMenu");
			this.m_OpenMenu.Name = "m_OpenMenu";
			this.m_OpenMenu.Click += new System.EventHandler(this.OnOpenClick);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			// 
			// m_Close
			// 
			this.m_Close.Name = "m_Close";
			resources.ApplyResources(this.m_Close, "m_Close");
			this.m_Close.Click += new System.EventHandler(this.m_Close_Click);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
			// 
			// m_PageSetup
			// 
			this.m_PageSetup.Image = global::LogStudio.Properties.Resources.PageSetup;
			resources.ApplyResources(this.m_PageSetup, "m_PageSetup");
			this.m_PageSetup.Name = "m_PageSetup";
			this.m_PageSetup.Click += new System.EventHandler(this.OnPageSetupClick);
			// 
			// m_PrintPreviewMenu
			// 
			this.m_PrintPreviewMenu.Image = global::LogStudio.Properties.Resources.Preview;
			resources.ApplyResources(this.m_PrintPreviewMenu, "m_PrintPreviewMenu");
			this.m_PrintPreviewMenu.Name = "m_PrintPreviewMenu";
			this.m_PrintPreviewMenu.Click += new System.EventHandler(this.OnPrintPreviewClick);
			// 
			// m_PrintMenu
			// 
			this.m_PrintMenu.Image = global::LogStudio.Properties.Resources.Print;
			resources.ApplyResources(this.m_PrintMenu, "m_PrintMenu");
			this.m_PrintMenu.Name = "m_PrintMenu";
			this.m_PrintMenu.Click += new System.EventHandler(this.OnPrintButtonClick);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
			// 
			// m_RecentFileHistory
			// 
			this.m_RecentFileHistory.Name = "m_RecentFileHistory";
			resources.ApplyResources(this.m_RecentFileHistory, "m_RecentFileHistory");
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.OnCloseClick);
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.openToolStripMenuItem,
            this.m_SettingsSave,
            this.toolStripMenuItem3});
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
			this.settingsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.settingsToolStripMenuItem_DropDownOpening);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Image = global::LogStudio.Properties.Resources.NewDocument;
			resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Click += new System.EventHandler(this.OnSettingsNewMenu);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Image = global::LogStudio.Properties.Resources.Open_folder;
			resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.OnSettingsOpenMenu);
			// 
			// m_SettingsSave
			// 
			this.m_SettingsSave.Image = global::LogStudio.Properties.Resources.Save;
			resources.ApplyResources(this.m_SettingsSave, "m_SettingsSave");
			this.m_SettingsSave.Name = "m_SettingsSave";
			this.m_SettingsSave.Click += new System.EventHandler(this.OnSettingsSaveMenu);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Image = global::LogStudio.Properties.Resources.Save;
			resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Click += new System.EventHandler(this.OnSettingsSaveAsMenu);
			// 
			// m_ToolsMenu
			// 
			this.m_ToolsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_Statistics,
            this.troubleshootingToolStripMenuItem});
			this.m_ToolsMenu.Name = "m_ToolsMenu";
			resources.ApplyResources(this.m_ToolsMenu, "m_ToolsMenu");
			this.m_ToolsMenu.DropDownOpening += new System.EventHandler(this.m_ToolsMenu_DropDownOpening);
			// 
			// m_Statistics
			// 
			this.m_Statistics.Name = "m_Statistics";
			resources.ApplyResources(this.m_Statistics, "m_Statistics");
			this.m_Statistics.Click += new System.EventHandler(this.m_Statistics_Click);
			// 
			// troubleshootingToolStripMenuItem
			// 
			this.troubleshootingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.compareLogFileToolStripMenuItem,
            this.beamCurrentCompensationToolStripMenuItem,
            this.correlationToolStripMenuItem});
			this.troubleshootingToolStripMenuItem.Name = "troubleshootingToolStripMenuItem";
			resources.ApplyResources(this.troubleshootingToolStripMenuItem, "troubleshootingToolStripMenuItem");
			// 
			// compareLogFileToolStripMenuItem
			// 
			this.compareLogFileToolStripMenuItem.Name = "compareLogFileToolStripMenuItem";
			resources.ApplyResources(this.compareLogFileToolStripMenuItem, "compareLogFileToolStripMenuItem");
			this.compareLogFileToolStripMenuItem.Click += new System.EventHandler(this.compareLogFileToolStripMenuItem_Click);
			// 
			// beamCurrentCompensationToolStripMenuItem
			// 
			resources.ApplyResources(this.beamCurrentCompensationToolStripMenuItem, "beamCurrentCompensationToolStripMenuItem");
			this.beamCurrentCompensationToolStripMenuItem.Name = "beamCurrentCompensationToolStripMenuItem";
			this.beamCurrentCompensationToolStripMenuItem.Click += new System.EventHandler(this.BeamCurrentCompensationToolStripMenuItem_Click);
			// 
			// correlationToolStripMenuItem
			// 
			this.correlationToolStripMenuItem.Name = "correlationToolStripMenuItem";
			resources.ApplyResources(this.correlationToolStripMenuItem, "correlationToolStripMenuItem");
			this.correlationToolStripMenuItem.Click += new System.EventHandler(this.correlationToolStripMenuItem_Click);
			// 
			// m_Admin
			// 
			this.m_Admin.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_FilterEditor});
			this.m_Admin.Name = "m_Admin";
			resources.ApplyResources(this.m_Admin, "m_Admin");
			this.m_Admin.DropDownOpening += new System.EventHandler(this.OnAdminOpening);
			// 
			// m_FilterEditor
			// 
			this.m_FilterEditor.Name = "m_FilterEditor";
			resources.ApplyResources(this.m_FilterEditor, "m_FilterEditor");
			this.m_FilterEditor.Click += new System.EventHandler(this.OnEditFilterClicked);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.OnShowAbout);
			// 
			// m_StatusStrip
			// 
			this.m_StatusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.m_StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_Progress,
            this.m_Task,
            this.m_Modified,
            this.m_UpdatingLabel,
            this.m_UpdatingProgress});
			resources.ApplyResources(this.m_StatusStrip, "m_StatusStrip");
			this.m_StatusStrip.Name = "m_StatusStrip";
			// 
			// m_Progress
			// 
			this.m_Progress.Name = "m_Progress";
			resources.ApplyResources(this.m_Progress, "m_Progress");
			// 
			// m_Task
			// 
			this.m_Task.Name = "m_Task";
			resources.ApplyResources(this.m_Task, "m_Task");
			// 
			// m_Modified
			// 
			this.m_Modified.Name = "m_Modified";
			resources.ApplyResources(this.m_Modified, "m_Modified");
			this.m_Modified.Spring = true;
			// 
			// m_UpdatingLabel
			// 
			this.m_UpdatingLabel.Name = "m_UpdatingLabel";
			resources.ApplyResources(this.m_UpdatingLabel, "m_UpdatingLabel");
			// 
			// m_UpdatingProgress
			// 
			this.m_UpdatingProgress.Name = "m_UpdatingProgress";
			resources.ApplyResources(this.m_UpdatingProgress, "m_UpdatingProgress");
			this.m_UpdatingProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			// 
			// m_DlgPrintPreview
			// 
			resources.ApplyResources(this.m_DlgPrintPreview, "m_DlgPrintPreview");
			this.m_DlgPrintPreview.Document = this.m_PrintDocument;
			this.m_DlgPrintPreview.Name = "m_DlgPrintPreview";
			// 
			// m_PrintDocument
			// 
			this.m_PrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.OnPrintDocumentPrintPage);
			// 
			// m_DlgPrint
			// 
			this.m_DlgPrint.Document = this.m_PrintDocument;
			this.m_DlgPrint.UseEXDialog = true;
			// 
			// m_DlgPageSetup
			// 
			this.m_DlgPageSetup.Document = this.m_PrintDocument;
			// 
			// m_ModifiedTimer
			// 
			this.m_ModifiedTimer.Enabled = true;
			this.m_ModifiedTimer.Interval = 250;
			this.m_ModifiedTimer.Tick += new System.EventHandler(this.OnModifiedTimerTick);
			// 
			// m_DlgSaveSettings
			// 
			this.m_DlgSaveSettings.DefaultExt = "*.lss";
			resources.ApplyResources(this.m_DlgSaveSettings, "m_DlgSaveSettings");
			// 
			// m_OpenDlg
			// 
			this.m_OpenDlg.DefaultExt = "plg";
			resources.ApplyResources(this.m_OpenDlg, "m_OpenDlg");
			this.m_OpenDlg.RestoreDirectory = true;
			// 
			// m_DlgOpenSettings
			// 
			this.m_DlgOpenSettings.DefaultExt = "*.lss";
			resources.ApplyResources(this.m_DlgOpenSettings, "m_DlgOpenSettings");
			// 
			// Form1
			// 
			this.AllowDrop = true;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.m_StatusStrip);
			this.Controls.Add(this.m_FileMenuStrip);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "Form1";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnDragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnDragEnter);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.m_FileMenuStrip.ResumeLayout(false);
			this.m_FileMenuStrip.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.m_StatusStrip.ResumeLayout(false);
			this.m_StatusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m_OpenMenu;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.StatusStrip m_StatusStrip;
        private System.Windows.Forms.TabControl m_ModuleTabControl;
        private System.Windows.Forms.ToolStripProgressBar m_Progress;
        private System.Windows.Forms.OpenFileDialog m_OpenDlg;
        private System.Windows.Forms.ToolStripStatusLabel m_Task;
        private System.Windows.Forms.ToolStrip m_FileMenuStrip;
        private System.Windows.Forms.ToolStripButton m_OpenButton;
        private System.Windows.Forms.ToolStripButton m_PrintButton;
        private System.Windows.Forms.ToolStripButton m_PrintPreview;
        private System.Windows.Forms.PrintPreviewDialog m_DlgPrintPreview;
        private System.Drawing.Printing.PrintDocument m_PrintDocument;
        private System.Windows.Forms.PrintDialog m_DlgPrint;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m_SettingsSave;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.SaveFileDialog m_DlgSaveSettings;
        private System.Windows.Forms.OpenFileDialog m_DlgOpenSettings;
        private System.Windows.Forms.ToolStripMenuItem m_PageSetup;
        private System.Windows.Forms.ToolStripMenuItem m_PrintMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ImageList m_TabImages;
        private System.Windows.Forms.PageSetupDialog m_DlgPageSetup;
        private System.Windows.Forms.ToolStripStatusLabel m_Modified;
        private System.Windows.Forms.Timer m_ModifiedTimer;
        private System.Windows.Forms.ToolStripMenuItem m_Admin;
        private System.Windows.Forms.ToolStripMenuItem m_FilterEditor;
        private System.Windows.Forms.ToolStripMenuItem m_ToolsMenu;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private ItemsTreeView m_ItemTree;
        private System.Windows.Forms.ToolStripMenuItem m_Statistics;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripLabel m_ModuleLabel;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel m_UpdatingLabel;
        private System.Windows.Forms.ToolStripProgressBar m_UpdatingProgress;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m_PrintPreviewMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem m_RecentFileHistory;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem m_Close;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem troubleshootingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compareLogFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem beamCurrentCompensationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem correlationToolStripMenuItem;
	}
}

