namespace LogStudio.Reader
{
    partial class CycleViewer
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
            this.m_List = new LogStudio.Reader.ListViewNf();
            this.colTimeStamp = new System.Windows.Forms.ColumnHeader();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.colValue = new System.Windows.Forms.ColumnHeader();
            this.colCycle = new System.Windows.Forms.ColumnHeader();
            this.colUser = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // m_List
            // 
            this.m_List.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colTimeStamp,
            this.colName,
            this.colValue,
            this.colCycle,
            this.colUser});
            this.m_List.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_List.FullRowSelect = true;
            this.m_List.HideSelection = false;
            this.m_List.LabelWrap = false;
            this.m_List.Location = new System.Drawing.Point(0, 0);
            this.m_List.Name = "m_List";
            this.m_List.OwnerDraw = true;
            this.m_List.Size = new System.Drawing.Size(737, 394);
            this.m_List.TabIndex = 1;
            this.m_List.UseCompatibleStateImageBehavior = false;
            this.m_List.View = System.Windows.Forms.View.Details;
            this.m_List.VirtualMode = true;
            this.m_List.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.m_List_DrawColumnHeader);
            this.m_List.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.m_List_RetrieveVirtualItem);
            this.m_List.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.m_List_DrawSubItem);
            // 
            // colTimeStamp
            // 
            this.colTimeStamp.Text = "Time Stamp";
            this.colTimeStamp.Width = 150;
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 200;
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
            // CycleViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(737, 394);
            this.Controls.Add(this.m_List);
            this.Name = "CycleViewer";
            this.Text = "CycleViewer";
            this.ResumeLayout(false);

        }

        #endregion

        private ListViewNf m_List;
        private System.Windows.Forms.ColumnHeader colTimeStamp;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colValue;
        private System.Windows.Forms.ColumnHeader colCycle;
        private System.Windows.Forms.ColumnHeader colUser;
    }
}