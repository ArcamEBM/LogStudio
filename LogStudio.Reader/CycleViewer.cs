using LogStudio.Data;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LogStudio.Reader
{
    public partial class CycleViewer : Form
    {
        private IItemDatabase m_Database;
        private readonly IDictionary<string, Color> m_ColorMap;

        public CycleViewer(IItemDatabase database, long uniqueID, IDictionary<string, Color> colorMap)
        {
            InitializeComponent();

            m_Database = database;
            m_ColorMap = colorMap;
            m_List.VirtualListSize = m_Database.Count;

            int index = database.GetRowIndex(uniqueID);
            if (index >= 0)
            {
                var item = m_List.Items[index];
                m_List.TopItem = item;
                m_List.FocusedItem = item;
                m_List.EnsureVisible(index);
            }

            m_StartTime = ((LogRowData)m_List.TopItem.Tag).OaTimeStamp;
        }

        private double m_StartTime = 0;

        private ListViewItem CreateItemFromIndex(LogRowData data)
        {
            ListViewItem item = new ListViewItem(data.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            item.SubItems.Add(data.ItemID);
            item.SubItems.Add(data.Value);
            item.SubItems.Add(data.CycleIndex.ToString());
            item.SubItems.Add(data.User);
            item.Tag = data;

            if (data.OaTimeStamp == m_StartTime)
            {
                item.BackColor = Color.Chartreuse;
                for (int i = 0; i < item.SubItems.Count; i++)
                    item.SubItems[i].BackColor = Color.Chartreuse;
            }
            else if (m_ColorMap.TryGetValue(data.ItemID, out var color))
            {
                item.BackColor = color;
                for (int i = 0; i < item.SubItems.Count; i++)
                    item.SubItems[i].BackColor = color;
            }

            return item;
        }

        private void m_List_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = CreateItemFromIndex(m_Database.GetRange(e.ItemIndex, 1).First());
        }

        private void m_List_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawText();

            if ((e.ItemState & ListViewItemStates.Selected) == ListViewItemStates.Selected)
            {
                e.DrawFocusRectangle(e.Item.Bounds);
            }

            if (e.ItemIndex > 0)
            {
                ListViewItem prevItem = m_List.Items[e.ItemIndex - 1];
                if (prevItem != null)
                {
                    if (((LogRowData)prevItem.Tag).CycleIndex != ((LogRowData)e.Item.Tag).CycleIndex)
                    {
                        e.Graphics.DrawLine(Pens.Black, e.SubItem.Bounds.Left, e.SubItem.Bounds.Top - 1, e.SubItem.Bounds.Right, e.SubItem.Bounds.Top - 1);
                    }
                }
            }
        }

        private void m_List_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }
    }
}
