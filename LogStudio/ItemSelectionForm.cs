using LogStudio.Data;
using System;
using System.Windows.Forms;

namespace LogStudio
{
    public partial class ItemSelectionForm : Form
    {
        public ItemSelectionForm()
        {
            InitializeComponent();
        }

        public string[] SelectedItems { get; private set; }

        public DialogResult ShowDialog(IItemDatabase database, string[] selectedItems)
        {
            m_Tree.ItemDatabase = database;

            return ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectedItems = m_Tree.GetCheckedItems();

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
