using LogStudio.Data;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LogStudio
{
    public partial class ItemInformationForm : Form
    {
        private readonly IItemDatabase m_Database;
        private readonly string m_ItemID;

        public ItemInformationForm(IItemDatabase database, string itemID)
        {
            m_Database = database;
            m_ItemID = itemID;

            InitializeComponent();

            m_Database.GetItemRange(m_ItemID, out DateTime from, out DateTime to);

            m_FromTime.Text = $"{from:d} {from:T}";
            //must be careful about truncating

            to = to.AddSeconds(1);
            m_ToTime.Text = $"{to:d} {to:T}";

            m_FromLabel.Text = "From:"; // string.Format("From: ({0:g})", from);
            m_ToLabel.Text = "To:"; // string.Format("To: ({0:g})", to);
        }

        public void UpdateValues()
        {
            m_list.Items.Clear();

            var info = new ItemInformation(m_ItemID, DateTime.Parse(m_FromTime.Text), DateTime.Parse(m_ToTime.Text));

            m_Database?.ForEach(m_ItemID, info.InfoCollect);

            m_ItemName.Text = info.ItemId;

            ListViewItem item = m_list.Items.Add("Min");
            if (info.Count != 0)
            {
                item.SubItems.Add(info.MinValue.ToString());
                item.SubItems.Add(info.MinValueTimeStamp.ToString());
            }
            else
            {
                item.SubItems.Add("n/a");
                item.SubItems.Add("n/a");
            }

            item = m_list.Items.Add("Max");
            if (info.Count != 0)
            {
                item.SubItems.Add(info.MaxValue.ToString());
                item.SubItems.Add(info.MaxValueTimeStamp.ToString());
            }
            else
            {
                item.SubItems.Add("n/a");
                item.SubItems.Add("n/a");
            }

            item = m_list.Items.Add("Count");
            item.SubItems.Add(info.Count.ToString());
        }

        private void ItemInformationForm_Load(object sender, EventArgs e)
        {

        }

        private void m_Refresh_Click(object sender, EventArgs e)
        {
            UpdateValues();
        }

        private void m_FromTime_Validating(object sender, CancelEventArgs e)
        {
            m_Database.GetItemRange(m_ItemID, out DateTime _, out DateTime _);

            var box = (TextBox)sender;

            bool failed = true;

            if (DateTime.TryParse(box.Text, out DateTime time))
            {
                box.Text = $"{time:d} {time:T}";

                failed = false;
                //if (time >= from && time <= to)
                //    failed = false;
            }

            if (failed)
            {
                e.Cancel = true;
                box.BackColor = Color.Salmon;
            }
            else
                box.BackColor = SystemColors.Window;
        }
    }
}