using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LogStudio
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();

            m_ReportsPath.Text = Properties.Settings.Default.UserStoragePath;
            m_LoadSettings.Checked = Properties.Settings.Default.LoadSettings;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.LoadSettings = m_LoadSettings.Checked;
            Properties.Settings.Default.UserStoragePath = m_ReportsPath.Text;
            Properties.Settings.Default.Save();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void m_ReportsPath_Validating(object sender, CancelEventArgs e)
        {
            if (!Directory.Exists(m_ReportsPath.Text))
            {
                e.Cancel = true;
                m_ReportsPath.BackColor = Color.Red;
                MessageBox.Show(Properties.Resources.RES_ReportPathNotValid, Application.ProductName);
            }
            else
            {
                m_ReportsPath.BackColor = SystemColors.Window;
            }
        }

        private void OnReportPathBrowseClick(object sender, EventArgs e)
        {
            m_DlgFolder.SelectedPath = Properties.Settings.Default.UserStoragePath;
            if (m_DlgFolder.ShowDialog() == DialogResult.OK)
            {
                m_ReportsPath.Text = m_DlgFolder.SelectedPath;
            }
        }
    }
}
