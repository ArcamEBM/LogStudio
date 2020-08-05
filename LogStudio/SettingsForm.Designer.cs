namespace LogStudio
{
    partial class SettingsForm
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
            this.m_TabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.m_MemorySaveMode = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.m_ReportsPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.m_DlgFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.m_LoadSettings = new System.Windows.Forms.CheckBox();
            this.m_TabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_TabControl
            // 
            this.m_TabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_TabControl.Controls.Add(this.tabPage1);
            this.m_TabControl.Location = new System.Drawing.Point(12, 12);
            this.m_TabControl.Name = "m_TabControl";
            this.m_TabControl.SelectedIndex = 0;
            this.m_TabControl.Size = new System.Drawing.Size(494, 170);
            this.m_TabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.m_LoadSettings);
            this.tabPage1.Controls.Add(this.m_MemorySaveMode);
            this.tabPage1.Controls.Add(this.button3);
            this.tabPage1.Controls.Add(this.m_ReportsPath);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(486, 144);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // m_MemorySaveMode
            // 
            this.m_MemorySaveMode.AutoSize = true;
            this.m_MemorySaveMode.Checked = global::LogStudio.Properties.Settings.Default.SaveMemory;
            this.m_MemorySaveMode.Location = new System.Drawing.Point(10, 51);
            this.m_MemorySaveMode.Name = "m_MemorySaveMode";
            this.m_MemorySaveMode.Size = new System.Drawing.Size(290, 17);
            this.m_MemorySaveMode.TabIndex = 3;
            this.m_MemorySaveMode.Text = "Enable memory save mode (Performace will be reduced)";
            this.toolTip1.SetToolTip(this.m_MemorySaveMode, "Check this checkbox to enable the memory save mode.\r\nThis will force the differen" +
                    "t views to release their cached data\r\nwhen they are inactive.");
            this.m_MemorySaveMode.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(447, 19);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(33, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "...";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.OnReportPathBrowseClick);
            // 
            // m_ReportsPath
            // 
            this.m_ReportsPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_ReportsPath.Location = new System.Drawing.Point(10, 21);
            this.m_ReportsPath.Name = "m_ReportsPath";
            this.m_ReportsPath.Size = new System.Drawing.Size(431, 20);
            this.m_ReportsPath.TabIndex = 1;
            this.toolTip1.SetToolTip(this.m_ReportsPath, "This is where all reports are stored.");
            this.m_ReportsPath.Validating += new System.ComponentModel.CancelEventHandler(this.m_ReportsPath_Validating);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Report file path";
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button1.Location = new System.Drawing.Point(181, 184);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "&Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(262, 184);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "&Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // m_LoadSettings
            // 
            this.m_LoadSettings.AutoSize = true;
            this.m_LoadSettings.Checked = global::LogStudio.Properties.Settings.Default.LoadSettings;
            this.m_LoadSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_LoadSettings.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::LogStudio.Properties.Settings.Default, "LoadSettings", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.m_LoadSettings.Location = new System.Drawing.Point(10, 74);
            this.m_LoadSettings.Name = "m_LoadSettings";
            this.m_LoadSettings.Size = new System.Drawing.Size(169, 17);
            this.m_LoadSettings.TabIndex = 4;
            this.m_LoadSettings.Text = "Load settings at startup";
            this.toolTip1.SetToolTip(this.m_LoadSettings, "Log Studio provides default graph configurations that are loaded at sta" +
                    "rtup unless this checkbox is unchecked.");
            this.m_LoadSettings.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(518, 219);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.m_TabControl);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            this.m_TabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl m_TabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox m_ReportsPath;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.FolderBrowserDialog m_DlgFolder;
        private System.Windows.Forms.CheckBox m_MemorySaveMode;
        private System.Windows.Forms.CheckBox m_LoadSettings;
    }
}