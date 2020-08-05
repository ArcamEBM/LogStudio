namespace LogStudio.Reader
{
    partial class LogReaderControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Current State", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Triggered by", System.Windows.Forms.HorizontalAlignment.Left);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.m_Copy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.m_CycleReaderButton = new System.Windows.Forms.ToolStripMenuItem();
            this.m_ShowStateView = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.m_SetMeasureStart = new System.Windows.Forms.ToolStripMenuItem();
            this.m_SetMeasureEnd = new System.Windows.Forms.ToolStripMenuItem();
            this.m_ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.m_SplitContainer = new System.Windows.Forms.SplitContainer();
            this.comboBoxComparisonOperators = new System.Windows.Forms.ComboBox();
            this.buttonResetFilter = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.m_List = new LogStudio.Reader.ListViewNf();
            this.colTimeStamp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colCycle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colUser = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.m_TabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.m_CurrentState = new LogStudio.Reader.ListViewNf();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.m_CallGraphPanel = new System.Windows.Forms.Panel();
            this.m_Measurements = new System.Windows.Forms.TabPage();
            this.m_TimingMeasurements = new LogStudio.Reader.TimingMeasurements();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_SplitContainer)).BeginInit();
            this.m_SplitContainer.Panel1.SuspendLayout();
            this.m_SplitContainer.Panel2.SuspendLayout();
            this.m_SplitContainer.SuspendLayout();
            this.m_TabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.m_Measurements.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_Copy,
            this.toolStripSeparator1,
            this.m_CycleReaderButton,
            this.m_ShowStateView,
            this.toolStripMenuItem1,
            this.m_SetMeasureStart,
            this.m_SetMeasureEnd});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(209, 126);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // m_Copy
            // 
            this.m_Copy.Name = "m_Copy";
            this.m_Copy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.m_Copy.Size = new System.Drawing.Size(208, 22);
            this.m_Copy.Text = "&Copy";
            this.m_Copy.Click += new System.EventHandler(this.m_Copy_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(205, 6);
            // 
            // m_CycleReaderButton
            // 
            this.m_CycleReaderButton.Name = "m_CycleReaderButton";
            this.m_CycleReaderButton.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
            this.m_CycleReaderButton.Size = new System.Drawing.Size(208, 22);
            this.m_CycleReaderButton.Text = "View Cycle Reader";
            this.m_CycleReaderButton.Click += new System.EventHandler(this.m_CycleReaderButton_Click);
            // 
            // m_ShowStateView
            // 
            this.m_ShowStateView.CheckOnClick = true;
            this.m_ShowStateView.Enabled = false;
            this.m_ShowStateView.Name = "m_ShowStateView";
            this.m_ShowStateView.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.m_ShowStateView.Size = new System.Drawing.Size(208, 22);
            this.m_ShowStateView.Text = "&Show State View";
            this.m_ShowStateView.CheckedChanged += new System.EventHandler(this.toolStripMenuItem2_CheckedChanged);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(205, 6);
            // 
            // m_SetMeasureStart
            // 
            this.m_SetMeasureStart.Enabled = false;
            this.m_SetMeasureStart.Name = "m_SetMeasureStart";
            this.m_SetMeasureStart.Size = new System.Drawing.Size(208, 22);
            this.m_SetMeasureStart.Text = "Set &Measure Start";
            this.m_SetMeasureStart.Click += new System.EventHandler(this.SetMeasureStartToolStripMenuItem_Click);
            // 
            // m_SetMeasureEnd
            // 
            this.m_SetMeasureEnd.Enabled = false;
            this.m_SetMeasureEnd.Name = "m_SetMeasureEnd";
            this.m_SetMeasureEnd.Size = new System.Drawing.Size(208, 22);
            this.m_SetMeasureEnd.Text = "Set Measure End";
            this.m_SetMeasureEnd.Click += new System.EventHandler(this.ToolStripMenuItem3_Click);
            // 
            // m_ToolTip
            // 
            this.m_ToolTip.IsBalloon = true;
            // 
            // m_SplitContainer
            // 
            this.m_SplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_SplitContainer.Location = new System.Drawing.Point(0, 0);
            this.m_SplitContainer.Name = "m_SplitContainer";
            this.m_SplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // m_SplitContainer.Panel1
            // 
            this.m_SplitContainer.Panel1.Controls.Add(this.comboBoxComparisonOperators);
            this.m_SplitContainer.Panel1.Controls.Add(this.buttonResetFilter);
            this.m_SplitContainer.Panel1.Controls.Add(this.label1);
            this.m_SplitContainer.Panel1.Controls.Add(this.textBoxFilter);
            this.m_SplitContainer.Panel1.Controls.Add(this.m_List);
            // 
            // m_SplitContainer.Panel2
            // 
            this.m_SplitContainer.Panel2.Controls.Add(this.m_TabControl);
            this.m_SplitContainer.Size = new System.Drawing.Size(1135, 569);
            this.m_SplitContainer.SplitterDistance = 396;
            this.m_SplitContainer.TabIndex = 1;
            // 
            // comboBoxComparisonOperators
            // 
            this.comboBoxComparisonOperators.FormattingEnabled = true;
            this.comboBoxComparisonOperators.Location = new System.Drawing.Point(284, 15);
            this.comboBoxComparisonOperators.Name = "comboBoxComparisonOperators";
            this.comboBoxComparisonOperators.Size = new System.Drawing.Size(131, 21);
            this.comboBoxComparisonOperators.TabIndex = 5;
            this.comboBoxComparisonOperators.SelectedIndexChanged += new System.EventHandler(this.ComboBoxComparisonOperators_SelectedIndexChanged);
            // 
            // buttonResetFilter
            // 
            this.buttonResetFilter.Location = new System.Drawing.Point(240, 14);
            this.buttonResetFilter.Name = "buttonResetFilter";
            this.buttonResetFilter.Size = new System.Drawing.Size(21, 20);
            this.buttonResetFilter.TabIndex = 3;
            this.buttonResetFilter.Text = "X";
            this.buttonResetFilter.UseVisualStyleBackColor = true;
            this.buttonResetFilter.Click += new System.EventHandler(this.ButtonResetFilter_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Value filter:";
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.Location = new System.Drawing.Point(96, 14);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(138, 20);
            this.textBoxFilter.TabIndex = 1;
            this.textBoxFilter.TextChanged += new System.EventHandler(this.TextBoxFilter_TextChanged);
            // 
            // m_List
            // 
            this.m_List.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_List.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colTimeStamp,
            this.colName,
            this.colValue,
            this.colCycle,
            this.colUser});
            this.m_List.ContextMenuStrip = this.contextMenuStrip1;
            this.m_List.FullRowSelect = true;
            this.m_List.HideSelection = false;
            this.m_List.LabelWrap = false;
            this.m_List.Location = new System.Drawing.Point(0, 51);
            this.m_List.Name = "m_List";
            this.m_List.Size = new System.Drawing.Size(1135, 345);
            this.m_List.TabIndex = 0;
            this.m_List.UseCompatibleStateImageBehavior = false;
            this.m_List.View = System.Windows.Forms.View.Details;
            this.m_List.VirtualMode = true;
            this.m_List.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.m_List_ColumnClick);
            this.m_List.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.m_List_ColumnWidthChanged);
            this.m_List.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.m_List_ColumnWidthChanging);
            this.m_List.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.m_List_RetrieveVirtualItem);
            this.m_List.SearchForVirtualItem += new System.Windows.Forms.SearchForVirtualItemEventHandler(this.m_List_SearchForVirtualItem);
            this.m_List.SelectedIndexChanged += new System.EventHandler(this.m_List_SelectedItemChanged);
            this.m_List.DoubleClick += new System.EventHandler(this.m_List_DoubleClick);
            this.m_List.KeyDown += new System.Windows.Forms.KeyEventHandler(this.m_List_KeyDown);
            // 
            // colTimeStamp
            // 
            this.colTimeStamp.Text = "Time Stamp";
            this.colTimeStamp.Width = 150;
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 300;
            // 
            // colValue
            // 
            this.colValue.Text = "Value";
            this.colValue.Width = 100;
            // 
            // colCycle
            // 
            this.colCycle.Text = "Cycle Index";
            this.colCycle.Width = 80;
            // 
            // colUser
            // 
            this.colUser.Text = "User";
            this.colUser.Width = 100;
            // 
            // m_TabControl
            // 
            this.m_TabControl.Controls.Add(this.tabPage1);
            this.m_TabControl.Controls.Add(this.tabPage2);
            this.m_TabControl.Controls.Add(this.m_Measurements);
            this.m_TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_TabControl.Location = new System.Drawing.Point(0, 0);
            this.m_TabControl.Name = "m_TabControl";
            this.m_TabControl.SelectedIndex = 0;
            this.m_TabControl.Size = new System.Drawing.Size(1135, 169);
            this.m_TabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.m_CurrentState);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1127, 143);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "State Viewer";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // m_CurrentState
            // 
            this.m_CurrentState.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.m_CurrentState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_CurrentState.FullRowSelect = true;
            listViewGroup1.Header = "Current State";
            listViewGroup1.Name = "currentStateGroup";
            listViewGroup2.Header = "Triggered by";
            listViewGroup2.Name = "triggersGroup";
            this.m_CurrentState.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
            this.m_CurrentState.HideSelection = false;
            this.m_CurrentState.Location = new System.Drawing.Point(3, 3);
            this.m_CurrentState.Name = "m_CurrentState";
            this.m_CurrentState.Size = new System.Drawing.Size(1121, 137);
            this.m_CurrentState.TabIndex = 0;
            this.m_CurrentState.UseCompatibleStateImageBehavior = false;
            this.m_CurrentState.View = System.Windows.Forms.View.Details;
            this.m_CurrentState.DoubleClick += new System.EventHandler(this.m_CurrentState_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Time Stamp";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 300;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Value";
            this.columnHeader3.Width = 100;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Cycle Index";
            this.columnHeader4.Width = 80;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "User";
            this.columnHeader5.Width = 100;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.m_CallGraphPanel);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1127, 143);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Call graph";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // m_CallGraphPanel
            // 
            this.m_CallGraphPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_CallGraphPanel.Location = new System.Drawing.Point(3, 3);
            this.m_CallGraphPanel.Name = "m_CallGraphPanel";
            this.m_CallGraphPanel.Size = new System.Drawing.Size(1121, 137);
            this.m_CallGraphPanel.TabIndex = 0;
            // 
            // m_Measurements
            // 
            this.m_Measurements.Controls.Add(this.m_TimingMeasurements);
            this.m_Measurements.Location = new System.Drawing.Point(4, 22);
            this.m_Measurements.Name = "m_Measurements";
            this.m_Measurements.Padding = new System.Windows.Forms.Padding(3);
            this.m_Measurements.Size = new System.Drawing.Size(1127, 143);
            this.m_Measurements.TabIndex = 2;
            this.m_Measurements.Text = "Measurements";
            this.m_Measurements.UseVisualStyleBackColor = true;
            // 
            // m_TimingMeasurements
            // 
            this.m_TimingMeasurements.Database = null;
            this.m_TimingMeasurements.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_TimingMeasurements.Location = new System.Drawing.Point(3, 3);
            this.m_TimingMeasurements.Name = "m_TimingMeasurements";
            this.m_TimingMeasurements.Size = new System.Drawing.Size(1121, 137);
            this.m_TimingMeasurements.TabIndex = 0;
            // 
            // LogReaderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_SplitContainer);
            this.Name = "LogReaderControl";
            this.Size = new System.Drawing.Size(1135, 569);
            this.contextMenuStrip1.ResumeLayout(false);
            this.m_SplitContainer.Panel1.ResumeLayout(false);
            this.m_SplitContainer.Panel1.PerformLayout();
            this.m_SplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_SplitContainer)).EndInit();
            this.m_SplitContainer.ResumeLayout(false);
            this.m_TabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.m_Measurements.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ListViewNf m_List;
        private System.Windows.Forms.ColumnHeader colTimeStamp;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colValue;
        private System.Windows.Forms.ColumnHeader colCycle;
        private System.Windows.Forms.ColumnHeader colUser;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem m_Copy;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem m_CycleReaderButton;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.SplitContainer m_SplitContainer;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ToolStripMenuItem m_ShowStateView;
        private ListViewNf m_CurrentState;
        private System.Windows.Forms.ToolTip m_ToolTip;
        private System.Windows.Forms.TabControl m_TabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel m_CallGraphPanel;
        private System.Windows.Forms.TabPage m_Measurements;
        private TimingMeasurements m_TimingMeasurements;
        private System.Windows.Forms.ToolStripMenuItem m_SetMeasureStart;
        private System.Windows.Forms.ToolStripMenuItem m_SetMeasureEnd;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.Button buttonResetFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxComparisonOperators;
    }
}
