using LogStudio.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace LogStudio
{
    public partial class Statistics : Form
    {
        ListViewColumnSorter m_Sorter;
        private IItemDatabase m_Database;
        public Statistics()
        {
            InitializeComponent();
        }

        public void Initialize(IItemDatabase database)
        {
            m_Database = database;
            m_Sorter = new ListViewColumnSorter(new NumericSorter());
            m_Sorter.SortColumn = 1;
            m_Sorter.Order = SortOrder.Descending;

            UpdateList();
        }

        private void UpdateList()
        {
            int itemCount = 0;
            long logPosts = 0;

            string filter = m_Filter.Text.ToLower().Trim();

            m_ItemsList.ListViewItemSorter = null;
            m_ItemsList.BeginUpdate();
            m_ItemsList.Items.Clear();
            foreach (string itemID in m_Database.GetItemsIDs())
            {
                if (filter.Length > 0 && !itemID.ToLower().Contains(filter))
                    continue;

                ListViewItem item = new ListViewItem(itemID);
                int posts = m_Database.GetItemRowCount(itemID);
                item.SubItems.Add(posts.ToString());
                m_ItemsList.Items.Add(item);
                itemCount++;
                logPosts += posts;
            }
            m_ItemsList.EndUpdate();

            m_ItemsList.ListViewItemSorter = m_Sorter;
            m_ItemsList.Sort();

            Text = Text + string.Format(" ({0} unique items, {1} log posts)", itemCount, logPosts);
        }

        private void m_ItemsList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            //if (m_Sorter.SortColumn != e.Column)
            //{
            //    m_Sorter.SortColumn = e.Column;
            //}
            //else
            //{
            //    if (m_Sorter.Order == SortOrder.Descending)
            //        m_Sorter.Order = SortOrder.Ascending;
            //    else
            //        m_Sorter.Order = SortOrder.Descending;
            //}
        }

        private void m_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void m_Filter_TextChanged(object sender, EventArgs e)
        {
            UpdateList();
        }
    }

    /// <summary>
    /// This class is an implementation of the 'IComparer' interface.
    /// </summary>
    public class ListViewColumnSorter : IComparer
    {
        /// <summary>
        /// Specifies the column to be sorted
        /// </summary>
        private int ColumnToSort;
        /// <summary>
        /// Specifies the order in which to sort (i.e. 'Ascending').
        /// </summary>
        private SortOrder OrderOfSort;
        /// <summary>
        /// Comparer to use
        /// </summary>
        private NumericSorter m_Comparer;

        /// <summary>
        /// Class constructor.  Initializes various elements
        /// </summary>
        public ListViewColumnSorter(NumericSorter comparer, Type typeToSort = null)
        {
            m_Comparer = comparer;
            if (typeToSort != null)
	            m_Comparer.TypeToSort = typeToSort;
            else
				m_Comparer.TypeToSort = typeof(int);
            // Initialize the column to '0'
            ColumnToSort = 0;

            // Initialize the sort order to 'none'
            OrderOfSort = SortOrder.Ascending;
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
        public int Compare(object x, object y)
        {
            int compareResult;

            // Cast the objects to be compared to ListViewItem objects
            string sx = ((ListViewItem)x).SubItems[ColumnToSort].Text;
            string sy = ((ListViewItem)y).SubItems[ColumnToSort].Text;

            compareResult = m_Comparer.Compare(sx, sy);
            // Calculate correct return value based on object comparison
            if (OrderOfSort == SortOrder.Ascending)
            {
                // Ascending sort is selected, return normal result of compare operation
                return compareResult;
            }
            else if (OrderOfSort == SortOrder.Descending)
            {
                // Descending sort is selected, return negative result of compare operation
                return (-compareResult);
            }
            else
            {
                // Return '0' to indicate they are equal
                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn
        {
            set
            {
                ColumnToSort = value;
            }
            get
            {
                return ColumnToSort;
            }
        }

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order
        {
            set
            {
                OrderOfSort = value;
            }
            get
            {
                return OrderOfSort;
            }
        }
    }

    public class NumericSorter : IComparer
    {
	    public Type TypeToSort { set; get; }
		#region IComparer Members

		public int Compare(object x, object y)
        {
            if (TypeToSort == typeof(double))
                return double.Parse(x.ToString()).CompareTo(double.Parse(y.ToString()));
            else
		        return long.Parse(x.ToString()).CompareTo(long.Parse(y.ToString()));
        }
        
        #endregion
    }
}
