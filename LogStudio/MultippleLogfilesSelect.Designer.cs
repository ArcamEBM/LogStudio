namespace LogStudio
{
    partial class MultippleLogfilesSelect
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
            this.m_FileList = new System.Windows.Forms.ListBox();
            this.m_OK = new System.Windows.Forms.Button();
            this.m_Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_FileList
            // 
            this.m_FileList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_FileList.FormattingEnabled = true;
            this.m_FileList.IntegralHeight = false;
            this.m_FileList.Location = new System.Drawing.Point(12, 11);
            this.m_FileList.Name = "m_FileList";
            this.m_FileList.Size = new System.Drawing.Size(268, 212);
            this.m_FileList.TabIndex = 0;
            this.m_FileList.SelectedIndexChanged += new System.EventHandler(this.m_FileList_SelectedIndexChanged);
            // 
            // m_OK
            // 
            this.m_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_OK.Enabled = false;
            this.m_OK.Location = new System.Drawing.Point(12, 231);
            this.m_OK.Name = "m_OK";
            this.m_OK.Size = new System.Drawing.Size(75, 23);
            this.m_OK.TabIndex = 1;
            this.m_OK.Text = "&OK";
            this.m_OK.UseVisualStyleBackColor = true;
            // 
            // m_Cancel
            // 
            this.m_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_Cancel.Location = new System.Drawing.Point(205, 231);
            this.m_Cancel.Name = "m_Cancel";
            this.m_Cancel.Size = new System.Drawing.Size(75, 23);
            this.m_Cancel.TabIndex = 2;
            this.m_Cancel.Text = "&Cancel";
            this.m_Cancel.UseVisualStyleBackColor = true;
            // 
            // MultippleLogfilesSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.m_Cancel);
            this.Controls.Add(this.m_OK);
            this.Controls.Add(this.m_FileList);
            this.Name = "MultippleLogfilesSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select log file...";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox m_FileList;
        private System.Windows.Forms.Button m_OK;
        private System.Windows.Forms.Button m_Cancel;
    }
}