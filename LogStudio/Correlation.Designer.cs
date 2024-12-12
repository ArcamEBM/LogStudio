namespace LogStudio
{
    partial class Correlation
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
			this.m_ItemsList = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.m_Close = new System.Windows.Forms.Button();
			this.m_Filter = new System.Windows.Forms.TextBox();
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.frameCycleWindowTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// m_ItemsList
			// 
			this.m_ItemsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_ItemsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
			this.m_ItemsList.HideSelection = false;
			this.m_ItemsList.Location = new System.Drawing.Point(16, 64);
			this.m_ItemsList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.m_ItemsList.Name = "m_ItemsList";
			this.m_ItemsList.Size = new System.Drawing.Size(685, 466);
			this.m_ItemsList.TabIndex = 0;
			this.m_ItemsList.UseCompatibleStateImageBehavior = false;
			this.m_ItemsList.View = System.Windows.Forms.View.Details;
			this.m_ItemsList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.m_ItemsList_ColumnClick);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Name";
			this.columnHeader1.Width = 300;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Changed After";
			this.columnHeader2.Width = 120;
			// 
			// m_Close
			// 
			this.m_Close.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.m_Close.Location = new System.Drawing.Point(309, 538);
			this.m_Close.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.m_Close.Name = "m_Close";
			this.m_Close.Size = new System.Drawing.Size(100, 28);
			this.m_Close.TabIndex = 1;
			this.m_Close.Text = "&Close";
			this.m_Close.UseVisualStyleBackColor = true;
			this.m_Close.Click += new System.EventHandler(this.m_Close_Click);
			// 
			// m_Filter
			// 
			this.m_Filter.Location = new System.Drawing.Point(16, 32);
			this.m_Filter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.m_Filter.Name = "m_Filter";
			this.m_Filter.Size = new System.Drawing.Size(439, 22);
			this.m_Filter.TabIndex = 2;
			this.m_Filter.TextChanged += new System.EventHandler(this.m_Filter_TextChanged);
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Changed Before";
			this.columnHeader3.Width = 120;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Changes After";
			this.columnHeader4.Width = 120;
			// 
			// frameCycleWindowTextBox
			// 
			this.frameCycleWindowTextBox.Location = new System.Drawing.Point(557, 32);
			this.frameCycleWindowTextBox.Margin = new System.Windows.Forms.Padding(4);
			this.frameCycleWindowTextBox.Name = "frameCycleWindowTextBox";
			this.frameCycleWindowTextBox.Size = new System.Drawing.Size(136, 22);
			this.frameCycleWindowTextBox.TabIndex = 3;
			this.frameCycleWindowTextBox.Text = "20";
			this.frameCycleWindowTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frameCycleWindowTextBox_KeyDown);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(554, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(134, 16);
			this.label1.TabIndex = 4;
			this.label1.Text = "Frame Cycle Window";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(36, 16);
			this.label2.TabIndex = 5;
			this.label2.Text = "Filter";
			// 
			// Correlation
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(719, 570);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.frameCycleWindowTextBox);
			this.Controls.Add(this.m_Filter);
			this.Controls.Add(this.m_Close);
			this.Controls.Add(this.m_ItemsList);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "Correlation";
			this.Text = "Correlation";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView m_ItemsList;
        private System.Windows.Forms.Button m_Close;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.TextBox m_Filter;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.TextBox frameCycleWindowTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
	}
}