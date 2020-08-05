using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LogStudio.Data;

namespace LogStudio
{
    partial class BeamCurrentCompensationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.GraphsSplitContainer = new System.Windows.Forms.SplitContainer();
            this.TimeSelectionListBox = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.GraphsSplitContainer)).BeginInit();
            this.GraphsSplitContainer.Panel2.SuspendLayout();
            this.GraphsSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // GraphsSplitContainer
            // 
            this.GraphsSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GraphsSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.GraphsSplitContainer.Name = "GraphsSplitContainer";
            // 
            // GraphsSplitContainer.Panel2
            // 
            this.GraphsSplitContainer.Panel2.Controls.Add(this.TimeSelectionListBox);
            this.GraphsSplitContainer.Size = new System.Drawing.Size(800, 450);
            this.GraphsSplitContainer.SplitterDistance = 518;
            this.GraphsSplitContainer.TabIndex = 0;
            // 
            // TimeSelectionListBox
            // 
            this.TimeSelectionListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TimeSelectionListBox.FormattingEnabled = true;
            this.TimeSelectionListBox.Location = new System.Drawing.Point(0, 0);
            this.TimeSelectionListBox.Name = "TimeSelectionListBox";
            this.TimeSelectionListBox.Size = new System.Drawing.Size(278, 450);
            this.TimeSelectionListBox.TabIndex = 0;
            // 
            // BeamCurrentCompensationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.GraphsSplitContainer);
            this.Name = "BeamCurrentCompensationForm";
            this.Text = "Beam Current Feedback Compensation";
            this.GraphsSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GraphsSplitContainer)).EndInit();
            this.GraphsSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SplitContainer GraphsSplitContainer;
        private ListBox TimeSelectionListBox;
    }
}