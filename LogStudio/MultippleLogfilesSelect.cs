using System;
using System.Windows.Forms;

namespace LogStudio
{
    public partial class MultippleLogfilesSelect : Form
    {
        public MultippleLogfilesSelect()
        {
            InitializeComponent();
        }

        public string Filename { get; private set; }

        public DialogResult ShowDialog(string[] filenames)
        {
            m_FileList.Items.AddRange(filenames);
            return ShowDialog();
        }

        private void m_FileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Filename = (string)m_FileList.Items[m_FileList.SelectedIndex];

            m_OK.Enabled = !string.IsNullOrEmpty(Filename);
        }
    }
}
