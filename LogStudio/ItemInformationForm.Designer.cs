namespace LogStudio
{
    partial class ItemInformationForm
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
            this.m_list = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.button1 = new System.Windows.Forms.Button();
            this.m_ItemName = new System.Windows.Forms.Label();
            this.m_Refresh = new System.Windows.Forms.Button();
            this.m_FromTime = new System.Windows.Forms.TextBox();
            this.m_ToTime = new System.Windows.Forms.TextBox();
            this.m_FromLabel = new System.Windows.Forms.Label();
            this.m_ToLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_list
            // 
            this.m_list.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.m_list.Location = new System.Drawing.Point(12, 65);
            this.m_list.Name = "m_list";
            this.m_list.Size = new System.Drawing.Size(357, 145);
            this.m_list.TabIndex = 0;
            this.m_list.UseCompatibleStateImageBehavior = false;
            this.m_list.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Time Stamp";
            this.columnHeader3.Width = 121;
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(153, 216);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "&Close";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // m_ItemName
            // 
            this.m_ItemName.AutoSize = true;
            this.m_ItemName.Location = new System.Drawing.Point(12, 9);
            this.m_ItemName.Name = "m_ItemName";
            this.m_ItemName.Size = new System.Drawing.Size(35, 13);
            this.m_ItemName.TabIndex = 2;
            this.m_ItemName.Text = "label1";
            // 
            // m_Refresh
            // 
            this.m_Refresh.Image = global::LogStudio.Properties.Resources.Search;
            this.m_Refresh.Location = new System.Drawing.Point(292, 32);
            this.m_Refresh.Name = "m_Refresh";
            this.m_Refresh.Size = new System.Drawing.Size(32, 32);
            this.m_Refresh.TabIndex = 5;
            this.m_Refresh.UseVisualStyleBackColor = true;
            this.m_Refresh.Click += new System.EventHandler(this.m_Refresh_Click);
            // 
            // m_FromTime
            // 
            this.m_FromTime.Location = new System.Drawing.Point(12, 39);
            this.m_FromTime.Name = "m_FromTime";
            this.m_FromTime.Size = new System.Drawing.Size(134, 20);
            this.m_FromTime.TabIndex = 6;
            this.m_FromTime.Validating += new System.ComponentModel.CancelEventHandler(this.m_FromTime_Validating);
            // 
            // m_ToTime
            // 
            this.m_ToTime.Location = new System.Drawing.Point(152, 39);
            this.m_ToTime.Name = "m_ToTime";
            this.m_ToTime.Size = new System.Drawing.Size(134, 20);
            this.m_ToTime.TabIndex = 7;
            this.m_ToTime.Validating += new System.ComponentModel.CancelEventHandler(this.m_FromTime_Validating);
            // 
            // m_FromLabel
            // 
            this.m_FromLabel.AutoSize = true;
            this.m_FromLabel.Location = new System.Drawing.Point(12, 26);
            this.m_FromLabel.Name = "m_FromLabel";
            this.m_FromLabel.Size = new System.Drawing.Size(33, 13);
            this.m_FromLabel.TabIndex = 8;
            this.m_FromLabel.Text = "From:";
            // 
            // m_ToLabel
            // 
            this.m_ToLabel.AutoSize = true;
            this.m_ToLabel.Location = new System.Drawing.Point(152, 26);
            this.m_ToLabel.Name = "m_ToLabel";
            this.m_ToLabel.Size = new System.Drawing.Size(23, 13);
            this.m_ToLabel.TabIndex = 9;
            this.m_ToLabel.Text = "To:";
            // 
            // ItemInformationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button1;
            this.ClientSize = new System.Drawing.Size(381, 248);
            this.Controls.Add(this.m_ToLabel);
            this.Controls.Add(this.m_FromLabel);
            this.Controls.Add(this.m_ToTime);
            this.Controls.Add(this.m_FromTime);
            this.Controls.Add(this.m_Refresh);
            this.Controls.Add(this.m_ItemName);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.m_list);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ItemInformationForm";
            this.ShowInTaskbar = false;
            this.Text = "Item Information";
            this.Load += new System.EventHandler(this.ItemInformationForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView m_list;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label m_ItemName;
        private System.Windows.Forms.Button m_Refresh;
        private System.Windows.Forms.TextBox m_FromTime;
        private System.Windows.Forms.TextBox m_ToTime;
        private System.Windows.Forms.Label m_FromLabel;
        private System.Windows.Forms.Label m_ToLabel;
    }
}