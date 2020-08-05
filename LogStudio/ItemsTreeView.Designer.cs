namespace LogStudio
{
    partial class ItemsTreeView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemsTreeView));
            this.m_Tree = new LogStudio.TreeViewEx();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.m_ExpandAllSubnodes = new System.Windows.Forms.ToolStripMenuItem();
            this.m_CollapsAllSubnodes = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.m_SelectAllSubitems = new System.Windows.Forms.ToolStripMenuItem();
            this.m_DeselectAllSubitems = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.m_ShowItemInformation = new System.Windows.Forms.ToolStripMenuItem();
            this.m_ExportToCSV = new System.Windows.Forms.ToolStripMenuItem();
            this.m_ExportStatesToCSV = new System.Windows.Forms.ToolStripMenuItem();
            this.m_ItemToClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.m_Images = new System.Windows.Forms.ImageList(this.components);
            this.m_DlgExportToCSV = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.m_RemoveSearch = new System.Windows.Forms.Button();
            this.m_Search = new System.Windows.Forms.Button();
            this.m_FilterText = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.m_DlgExportStatesToCSV = new System.Windows.Forms.SaveFileDialog();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_Tree
            // 
            this.m_Tree.CheckBoxes = true;
            this.m_Tree.ContextMenuStrip = this.contextMenuStrip1;
            this.m_Tree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_Tree.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.m_Tree.HideSelection = false;
            this.m_Tree.ImageIndex = 0;
            this.m_Tree.ImageList = this.m_Images;
            this.m_Tree.Location = new System.Drawing.Point(0, 49);
            this.m_Tree.Name = "m_Tree";
            this.m_Tree.PreventRepaint = false;
            this.m_Tree.RaiseAfterCheckEvent = true;
            this.m_Tree.SelectedImageIndex = 1;
            this.m_Tree.ShowLines = false;
            this.m_Tree.ShowRootLines = false;
            this.m_Tree.Size = new System.Drawing.Size(337, 240);
            this.m_Tree.TabIndex = 0;
            this.m_Tree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.m_Tree_AfterCheck);
            this.m_Tree.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.m_Tree_DrawNode);
            this.m_Tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.m_Tree_AfterSelect);
            this.m_Tree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.m_Tree_NodeMouseDoubleClick);
            this.m_Tree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.m_Tree_MouseDown);
            this.m_Tree.MouseUp += new System.Windows.Forms.MouseEventHandler(this.m_Tree_MouseUp);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_ExpandAllSubnodes,
            this.m_CollapsAllSubnodes,
            this.toolStripSeparator2,
            this.m_SelectAllSubitems,
            this.m_DeselectAllSubitems,
            this.toolStripSeparator1,
            this.m_ShowItemInformation,
            this.m_ExportToCSV,
            this.m_ExportStatesToCSV,
            this.m_ItemToClipboard});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(206, 192);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // m_ExpandAllSubnodes
            // 
            this.m_ExpandAllSubnodes.Name = "m_ExpandAllSubnodes";
            this.m_ExpandAllSubnodes.Size = new System.Drawing.Size(205, 22);
            this.m_ExpandAllSubnodes.Text = "&Expand all sub nodes";
            this.m_ExpandAllSubnodes.Click += new System.EventHandler(this.OnExpandAllSubnodes);
            // 
            // m_CollapsAllSubnodes
            // 
            this.m_CollapsAllSubnodes.Name = "m_CollapsAllSubnodes";
            this.m_CollapsAllSubnodes.Size = new System.Drawing.Size(205, 22);
            this.m_CollapsAllSubnodes.Text = "C&ollapse all sub nodes";
            this.m_CollapsAllSubnodes.Click += new System.EventHandler(this.m_CollapsAllSubnodes_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(202, 6);
            // 
            // m_SelectAllSubitems
            // 
            this.m_SelectAllSubitems.Name = "m_SelectAllSubitems";
            this.m_SelectAllSubitems.Size = new System.Drawing.Size(205, 22);
            this.m_SelectAllSubitems.Text = "Select &all sub items";
            this.m_SelectAllSubitems.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // m_DeselectAllSubitems
            // 
            this.m_DeselectAllSubitems.Name = "m_DeselectAllSubitems";
            this.m_DeselectAllSubitems.Size = new System.Drawing.Size(205, 22);
            this.m_DeselectAllSubitems.Text = "&Deselect all sub items";
            this.m_DeselectAllSubitems.Click += new System.EventHandler(this.OnDeselectAllSubitems);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(202, 6);
            // 
            // m_ShowItemInformation
            // 
            this.m_ShowItemInformation.Name = "m_ShowItemInformation";
            this.m_ShowItemInformation.Size = new System.Drawing.Size(205, 22);
            this.m_ShowItemInformation.Text = "&Show item information...";
            this.m_ShowItemInformation.Click += new System.EventHandler(this.OnShowItemInformation);
            // 
            // m_ExportToCSV
            // 
            this.m_ExportToCSV.Name = "m_ExportToCSV";
            this.m_ExportToCSV.Size = new System.Drawing.Size(205, 22);
            this.m_ExportToCSV.Text = "Export to &CSV...";
            this.m_ExportToCSV.Click += new System.EventHandler(this.OnExportToCSV);
            // 
            // m_ExportStatesToCSV
            // 
            this.m_ExportStatesToCSV.Name = "m_ExportStatesToCSV";
            this.m_ExportStatesToCSV.Size = new System.Drawing.Size(205, 22);
            this.m_ExportStatesToCSV.Text = "Export states to CSV...";
            this.m_ExportStatesToCSV.Click += new System.EventHandler(this.m_ExportStatesToCSV_Click);
            // 
            // m_ItemToClipboard
            // 
            this.m_ItemToClipboard.Name = "m_ItemToClipboard";
            this.m_ItemToClipboard.Size = new System.Drawing.Size(205, 22);
            this.m_ItemToClipboard.Text = "Item &name to clipboard";
            this.m_ItemToClipboard.Click += new System.EventHandler(this.itemNameToClipboardToolStripMenuItem_Click);
            // 
            // m_Images
            // 
            this.m_Images.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("m_Images.ImageStream")));
            this.m_Images.TransparentColor = System.Drawing.Color.Fuchsia;
            this.m_Images.Images.SetKeyName(0, "XPfolder_closed.bmp");
            this.m_Images.Images.SetKeyName(1, "XPfolder_Open.bmp");
            this.m_Images.Images.SetKeyName(2, "VSProject_genericfile.bmp");
            // 
            // m_DlgExportToCSV
            // 
            this.m_DlgExportToCSV.DefaultExt = "csv";
            this.m_DlgExportToCSV.Filter = "CSV-File (*.csv)|*.csv";
            this.m_DlgExportToCSV.Title = "Export to CSV...";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.m_RemoveSearch);
            this.groupBox1.Controls.Add(this.m_Search);
            this.groupBox1.Controls.Add(this.m_FilterText);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(337, 49);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search";
            // 
            // m_RemoveSearch
            // 
            this.m_RemoveSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_RemoveSearch.Enabled = false;
            this.m_RemoveSearch.Image = global::LogStudio.Properties.Resources.SearchCancel;
            this.m_RemoveSearch.Location = new System.Drawing.Point(302, 12);
            this.m_RemoveSearch.Name = "m_RemoveSearch";
            this.m_RemoveSearch.Size = new System.Drawing.Size(32, 32);
            this.m_RemoveSearch.TabIndex = 2;
            this.toolTip1.SetToolTip(this.m_RemoveSearch, "Clear the search string.");
            this.m_RemoveSearch.UseVisualStyleBackColor = true;
            this.m_RemoveSearch.Click += new System.EventHandler(this.OnRemoveSearch);
            // 
            // m_Search
            // 
            this.m_Search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_Search.Image = global::LogStudio.Properties.Resources.Search;
            this.m_Search.Location = new System.Drawing.Point(264, 12);
            this.m_Search.Name = "m_Search";
            this.m_Search.Size = new System.Drawing.Size(32, 32);
            this.m_Search.TabIndex = 1;
            this.toolTip1.SetToolTip(this.m_Search, "Start searching for items matching the search string.");
            this.m_Search.UseVisualStyleBackColor = true;
            this.m_Search.Click += new System.EventHandler(this.OnSearch);
            // 
            // m_FilterText
            // 
            this.m_FilterText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_FilterText.Location = new System.Drawing.Point(6, 19);
            this.m_FilterText.Name = "m_FilterText";
            this.m_FilterText.Size = new System.Drawing.Size(252, 20);
            this.m_FilterText.TabIndex = 0;
            this.toolTip1.SetToolTip(this.m_FilterText, "Enter the find criteria to show only relevant items in the tree.");
            this.m_FilterText.TextChanged += new System.EventHandler(this.m_FilterText_TextChanged);
            this.m_FilterText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.m_FilterText_KeyDown);
            // 
            // m_DlgExportStatesToCSV
            // 
            this.m_DlgExportStatesToCSV.DefaultExt = "csv";
            this.m_DlgExportStatesToCSV.Filter = "CSV-File (*.csv)|*.csv";
            this.m_DlgExportStatesToCSV.Title = "Export states to CSV...";
            // 
            // ItemsTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_Tree);
            this.Controls.Add(this.groupBox1);
            this.Name = "ItemsTreeView";
            this.Size = new System.Drawing.Size(337, 289);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TreeViewEx m_Tree;
        private System.Windows.Forms.ImageList m_Images;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem m_ShowItemInformation;
        private System.Windows.Forms.ToolStripMenuItem m_ExportToCSV;
        private System.Windows.Forms.SaveFileDialog m_DlgExportToCSV;
        private System.Windows.Forms.ToolStripMenuItem m_ItemToClipboard;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button m_Search;
        private System.Windows.Forms.TextBox m_FilterText;
        private System.Windows.Forms.ToolStripMenuItem m_CollapsAllSubnodes;
        private System.Windows.Forms.ToolStripMenuItem m_ExpandAllSubnodes;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem m_DeselectAllSubitems;
        private System.Windows.Forms.Button m_RemoveSearch;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem m_SelectAllSubitems;
        private System.Windows.Forms.ToolStripMenuItem m_ExportStatesToCSV;
        private System.Windows.Forms.SaveFileDialog m_DlgExportStatesToCSV;
    }
}
