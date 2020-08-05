namespace LogStudio
{
    partial class LogGraphControl
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
            this.m_TabControl = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // m_TabControl
            // 
            this.m_TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_TabControl.Location = new System.Drawing.Point(0, 0);
            this.m_TabControl.Name = "m_TabControl";
            this.m_TabControl.SelectedIndex = 0;
            this.m_TabControl.Size = new System.Drawing.Size(547, 292);
            this.m_TabControl.TabIndex = 1;
            this.m_TabControl.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.m_TabControl_Selecting);
            this.m_TabControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.m_TabControl_Selected);
            // 
            // LogGraphControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_TabControl);
            this.Name = "LogGraphControl";
            this.Size = new System.Drawing.Size(547, 292);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl m_TabControl;
    }
}
