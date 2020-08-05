using System;
using System.Windows.Forms;

namespace LogStudio.Framework
{
    public partial class ExceptionBox : Form
    {
        public ExceptionBox()
        {
            InitializeComponent();
        }

        private void m_Expand_Click(object sender, EventArgs e)
        {
            Size = MaximumSize;
        }

        private void ExceptionBox_Load(object sender, EventArgs e)
        {

        }

        public static void Show(Exception ex)
        {
            using (ExceptionBox win = new ExceptionBox())
            {
                win.m_Message.Text = ex.Message;
                win.m_StackTrace.Text = ex.ToString();
                win.ShowDialog();
            }
        }
    }
}
