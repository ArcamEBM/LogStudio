namespace LogStudio.Framework
{
    partial class ExceptionBox
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
            this.m_Ok = new System.Windows.Forms.Button();
            this.m_Message = new System.Windows.Forms.Label();
            this.m_StackTrace = new System.Windows.Forms.TextBox();
            this.m_Expand = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_Ok
            // 
            this.m_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_Ok.Location = new System.Drawing.Point(160, 67);
            this.m_Ok.Name = "m_Ok";
            this.m_Ok.Size = new System.Drawing.Size(75, 23);
            this.m_Ok.TabIndex = 0;
            this.m_Ok.Text = "&Ok";
            this.m_Ok.UseVisualStyleBackColor = true;
            // 
            // m_Message
            // 
            this.m_Message.Location = new System.Drawing.Point(13, 13);
            this.m_Message.Name = "m_Message";
            this.m_Message.Size = new System.Drawing.Size(359, 41);
            this.m_Message.TabIndex = 1;
            this.m_Message.Text = "label1";
            // 
            // m_StackTrace
            // 
            this.m_StackTrace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_StackTrace.Location = new System.Drawing.Point(13, 58);
            this.m_StackTrace.Multiline = true;
            this.m_StackTrace.Name = "m_StackTrace";
            this.m_StackTrace.ReadOnly = true;
            this.m_StackTrace.Size = new System.Drawing.Size(369, 3);
            this.m_StackTrace.TabIndex = 2;
            // 
            // m_Expand
            // 
            this.m_Expand.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_Expand.Location = new System.Drawing.Point(302, 67);
            this.m_Expand.Name = "m_Expand";
            this.m_Expand.Size = new System.Drawing.Size(75, 23);
            this.m_Expand.TabIndex = 3;
            this.m_Expand.Text = "&>>";
            this.m_Expand.UseVisualStyleBackColor = true;
            this.m_Expand.Click += new System.EventHandler(this.m_Expand_Click);
            // 
            // ExceptionBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 94);
            this.Controls.Add(this.m_Expand);
            this.Controls.Add(this.m_StackTrace);
            this.Controls.Add(this.m_Message);
            this.Controls.Add(this.m_Ok);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximumSize = new System.Drawing.Size(400, 300);
            this.MinimumSize = new System.Drawing.Size(400, 120);
            this.Name = "ExceptionBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ExceptionBox";
            this.Load += new System.EventHandler(this.ExceptionBox_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_Ok;
        private System.Windows.Forms.Label m_Message;
        private System.Windows.Forms.TextBox m_StackTrace;
        private System.Windows.Forms.Button m_Expand;
    }
}