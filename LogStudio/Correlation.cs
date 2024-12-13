using LogStudio.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LogStudio
{
	public struct CorrelationItem
	{
		public string ItemID;
		public double ChangedAfter;
		public double ChangedBefore;
		public double ChangesAfter;
	}

    public partial class Correlation : Form
    {
        ListViewColumnSorter m_Sorter;
        private IItemDatabase m_Database;
        private List<CorrelationItem> m_CorrelationList = new List<CorrelationItem>();
        private int m_FrameCycleWindow = 20;
        private string m_selectedItemID;
        public Correlation()
        {
            InitializeComponent();
        }

        public void Initialize(IItemDatabase database, string selectedItemID)
        {
            m_Database = database;
            m_Sorter = new ListViewColumnSorter(new NumericSorter(), typeof(double))
            {
	            SortColumn = 1,
	            Order = SortOrder.Descending
            };
            m_selectedItemID = selectedItemID;
            this.Text = "Correlation for " + selectedItemID;

			CreateCorrelationList(m_selectedItemID);
            UpdateList();
        }

        private void CreateCorrelationList(string selectedItemId)
        {
			m_CorrelationList.Clear();
			LogRowData[] selectedItemLog = m_Database.GetAll(selectedItemId).ToArray();
	        if (selectedItemLog.Length < 2) return;

			foreach (string itemID in m_Database.GetItemsIDs())
	        {
		        ItemProperties properties = m_Database.GetItemProperties(itemID);

		        if (properties == null)
                    continue;
		        
		        properties.TryGetValue("Type", out string typeValue);
		        if (typeValue == null)
                    continue;

		        LogRowData[] itemLog = m_Database.GetAll(itemID).ToArray();
                if (itemLog.Length < 2) continue;

                int i = 1;
                int j = 1;
                long selectedItemCycleIndex = selectedItemLog[i].CycleIndex;
                long itemCycleIndex = itemLog[j].CycleIndex;
                bool[] changeWithinWindow = new bool[selectedItemLog.Length];
                bool[] changesWithinWindow = new bool[itemLog.Length];
                while (j < itemLog.Count())
                {
	                if (selectedItemCycleIndex > itemCycleIndex)
	                {
		                if (selectedItemCycleIndex - itemCycleIndex < m_FrameCycleWindow)
							changeWithinWindow[i] = true;

						j++;
		                itemCycleIndex = j >= itemLog.Count() ? long.MaxValue : itemLog[j].CycleIndex;
					}
                    else if (selectedItemCycleIndex < itemCycleIndex)
	                {
                        if (itemCycleIndex - selectedItemCycleIndex < m_FrameCycleWindow)
                            changesWithinWindow[j] = true;

		                i++;
		                selectedItemCycleIndex = i >= selectedItemLog.Count() ? long.MaxValue : selectedItemLog[i].CycleIndex;
					}
                    else
                    {
	                    changeWithinWindow[i] = true;
	                    changesWithinWindow[j] = true;
						i++;
						selectedItemCycleIndex = i >= selectedItemLog.Count() ? long.MaxValue : selectedItemLog[i].CycleIndex;
						j++;
						itemCycleIndex = j >= itemLog.Count() ? long.MaxValue : itemLog[j].CycleIndex;
					}
		        }

                double numberOfTriggeredChanges = changeWithinWindow.Sum(c => c ? 1 : 0);
                double numberOfChangesAfter = changesWithinWindow.Sum(c => c ? 1 : 0);
                if (numberOfTriggeredChanges > 0 || numberOfChangesAfter > 0)
                {
	                CorrelationItem correlation = new CorrelationItem
	                {
		                ItemID = itemID,
		                ChangedAfter = numberOfTriggeredChanges / (itemLog.Length - 1),
		                ChangedBefore = numberOfTriggeredChanges / (selectedItemLog.Length - 1),
                        ChangesAfter = numberOfChangesAfter / (selectedItemLog.Length - 1)
					};
	                m_CorrelationList.Add(correlation);
                }
	        }
		}

        private void UpdateList()
        {
            string filter = m_Filter.Text.ToLower().Trim();

            m_ItemsList.ListViewItemSorter = null;
            m_ItemsList.BeginUpdate();
            m_ItemsList.Items.Clear();
            foreach (CorrelationItem correlationItem in m_CorrelationList)
            {
                if (filter.Length > 0 && !correlationItem.ItemID.ToLower().Contains(filter))
                    continue;

                ListViewItem item = new ListViewItem(correlationItem.ItemID);
                item.SubItems.Add(correlationItem.ChangedAfter.ToString());
                item.SubItems.Add(correlationItem.ChangedBefore.ToString());
                item.SubItems.Add(correlationItem.ChangesAfter.ToString());
                m_ItemsList.Items.Add(item);
            }
            m_ItemsList.EndUpdate();

            m_ItemsList.ListViewItemSorter = m_Sorter;
            m_ItemsList.Sort();
        }

        private void m_ItemsList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            m_Sorter.SortColumn = e.Column;
            m_ItemsList.Sort();
        }

        private void m_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void m_Filter_TextChanged(object sender, EventArgs e)
        {
            UpdateList();
        }

		private void frameCycleWindowTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				try
				{
					int newWindow = int.Parse(frameCycleWindowTextBox.Text);
                    m_FrameCycleWindow = newWindow;
                    CreateCorrelationList(m_selectedItemID);
                    UpdateList();
				}
				catch (FormatException exception)
				{
				}
			}
		}
	}
}
