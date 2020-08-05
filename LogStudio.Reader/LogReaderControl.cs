using JetBrains.Annotations;
using LogStudio.Data;
using LogStudio.Framework;
using LogStudio.Reader.Parser;
using Microsoft.Msagl.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace LogStudio.Reader
{
    public partial class LogReaderControl : UserControl, IComparer<LogRowKey> //, IModule
    {
        private enum SortColumn
        {
            TimeStamp,
            Name,
            Value,
            Cycle,
            User
        }

        private enum SortOrder
        {
            Ascending,
            Descending
        }

        private SortColumn m_SortColumn = SortColumn.TimeStamp;
        private SortOrder m_SortOrder = SortOrder.Ascending;

        /// <summary>
        /// This dictionary is used to add items dynamically to the list.
        /// </summary>
        private readonly Dictionary<string, int> m_ItemCount = new Dictionary<string, int>();

        /// <summary>
        /// This list is the keys to each item a row in the list represents.
        /// </summary>
        private readonly List<LogRowKey> m_RowKeys = new List<LogRowKey>();

        /// <summary>
        /// Contains the id of the currently selected item.
        /// </summary>
        private string m_SelectedItem = string.Empty;

        private readonly Dictionary<string, Color> m_ColorMap = new Dictionary<string, Color>();
        private readonly Microsoft.Msagl.GraphViewerGdi.GViewer m_Viewer;

        /// <summary>
        /// Private class for comparison operators in combo box
        /// </summary>
        private class ValueComparison
        {
            private readonly string m_Text;

            public Func<string, decimal, bool> Predicate { get; }

            public ValueComparison(string text, Func<string, decimal, bool> predicate)
            {
                m_Text = text;
                Predicate = predicate;
            }

            public override string ToString()
            {
                return m_Text;
            }
        }

        public LogReaderControl()
        {
            InitializeComponent();

            m_Culture = new CultureInfo("en-US");

            m_Viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer
            {
                Dock = DockStyle.Fill,
                LayoutEditingEnabled = false
            };
            m_CallGraphPanel.Controls.Add(m_Viewer);

            m_Viewer.ObjectUnderMouseCursorChanged += (sender, args) =>
            {
                if (args.NewObject?.DrawingObject.UserData == null)
                {
                    m_Viewer.SetToolTip(m_ToolTip, null);
                    return;
                }

                if (args.NewObject.DrawingObject.UserData is Change change)
                {
                    if (change.Result != null)
                        m_Viewer.SetToolTip(m_ToolTip,
                            $"{change.Result.Expression}\r\n{change.Result.BlockName}.{change.Result.MethodName}\r\n{change.Key.Data.ItemID}\r\n{change.Key.Data.CycleIndex}");
                    else
                        m_Viewer.SetToolTip(m_ToolTip,
                            $"Set by client\r\n{change.Key.Data.ItemID}\r\n{change.Key.Data.CycleIndex}");
                }
                else
                    m_Viewer.SetToolTip(m_ToolTip, null);
            };

            m_Viewer.MouseDoubleClick += (sender, args) =>
            {
                if (m_Viewer.SelectedObject is Node node)
                {
                    if (node.UserData is Change change)
                    {
                        SelectListItemFromRowKey(change.Key);
                    }
                }
            };

            m_TimingMeasurements.OnPointClicked += (sender, args) =>
            {
                int index = m_RowKeys.FindIndex(row =>
                    row.Data.ItemID == args.StartItemId && row.Data.TimeStamp == args.Timestamp);
                if (index >= 0)
                    SelectListItemFromRowKey(m_RowKeys[index]);
            };
            //RetrieveVirtualItem += new RetrieveVirtualItemEventHandler(listView1_RetrieveVirtualItem);
            //CacheVirtualItems += new CacheVirtualItemsEventHandler(listView1_CacheVirtualItems);
            //SearchForVirtualItem += new SearchForVirtualItemEventHandler(listView1_SearchForVirtualItem);

            m_SplitContainer.Panel2Collapsed = true;

            // Adding comparison operators
            this.comboBoxComparisonOperators.Items.AddRange(new object[]
            {
                new ValueComparison("None", (value, constant) => throw new InvalidOperationException()),
                new ValueComparison("==", (value, constant) =>
                {
                    if (!TryParseDecimalInvariant(value, out decimal decimalValue))
                        return false;

                    return decimalValue == constant;
                }),
                new ValueComparison("!=", (value, constant) =>
                {
                    if (!TryParseDecimalInvariant(value, out decimal decimalValue))
                        return false;

                    return decimalValue != constant;
                }),
                new ValueComparison(">", (value, constant) =>
                {
                    if (!TryParseDecimalInvariant(value, out decimal decimalValue))
                        return false;

                    return decimalValue > constant;
                }),
                new ValueComparison(">=", (value, constant) =>
                {
                    if (!TryParseDecimalInvariant(value, out decimal decimalValue))
                        return false;

                    return decimalValue >= constant;
                }),
                new ValueComparison("<", (value, constant) =>
                {
                    if (!TryParseDecimalInvariant(value, out decimal decimalValue))
                        return false;

                    return decimalValue < constant;
                }),
                new ValueComparison("<=", (value, constant) =>
                {
                    if (!TryParseDecimalInvariant(value, out decimal decimalValue))
                        return false;

                    return decimalValue <= constant;
                })
            });
        }

        public bool IsTheme = false;

        private void OnIndexesChanged(object sender, LogIndexChangesEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<LogIndexChangesEventArgs>(OnIndexesChanged), sender, e);
                return;
            }

            int isLastVisible = m_List.SelectedIndices.Count == 1 ? m_List.SelectedIndices[0] : -1;

            bool isLastItemSelected = isLastVisible == m_List.VirtualListSize - 1;

            foreach (LogIndexChangeRange item in e.Changes)
            {
                if (!m_ItemCount.TryGetValue(item.ItemID, out int itemsInList))
                    continue;

                int fromIndex = itemsInList;
                int length = item.Index + item.Count - itemsInList;

                if (length <= 0)
                    continue;

                long[] rowUniqueIDs = m_Database.GetLogRowUniqueID(item.ItemID, fromIndex, length);
                LogRowData[] logRowDatas = m_Database.GetItemRows(item.ItemID, fromIndex, length);
                for (int index = 0; index < rowUniqueIDs.Length; index++)
                {
                    m_RowKeys.Add(new LogRowKey(rowUniqueIDs[index], logRowDatas[index]));
                }

                m_ItemCount[item.ItemID] = itemsInList + item.Count;
            }

            m_RowKeys.Sort(this);

            m_List.VirtualListSize = m_RowKeys.Count;

            if (isLastItemSelected && m_List.VirtualListSize > 0)
            {
                ListViewItem lastItem = m_List.Items[m_List.VirtualListSize - 1];
                lastItem.EnsureVisible();
                m_List.SelectedIndices.Clear();
                lastItem.Selected = true;
            }
        }

        //public void ClearCachedItems()
        //{
        //    m_CacheFirstIndex = -1;
        //    m_CachedItems = null;
        //}
        /// <summary>
        /// Current item. Null if no items exist
        /// </summary>
        /// <returns></returns>
        private ListViewItem GetCurrentItem()
        {
            if (m_List.SelectedIndices.Count == 1)
            {
                return m_List.Items[m_List.SelectedIndices[0]];
            }

            if (m_List.VirtualListSize == 0)
                return null;

            int topIndex = m_List.TopItem.Index;
            double nbrItems = NumberOfVisibleItems;
            var getIndex = (int)Math.Round(topIndex + nbrItems * 0.5);
            int index = Math.Min(getIndex, m_List.VirtualListSize - 1);
            ListViewItem selectedItem = m_List.Items[index];
            return selectedItem;
        }

        private double NumberOfVisibleItems
        {
            get { return m_List.Height / (double)m_List.TopItem.Bounds.Height; }
        }

        private void AddItem(string itemId)
        {
            ListViewItem item = GetCurrentItem();

            //ClearCachedItems();
            if (m_Database == null)
                return;

            Debug.WriteLine($"Add item {itemId}");

            int count = m_Database.GetItemRowCount(itemId);

            long[] rowUniqueIDs = m_Database.GetLogRowUniqueID(itemId, 0, count);
            LogRowData[] logRowDatas = m_Database.GetItemRows(itemId, 0, count);

            string filter = this.textBoxFilter.Text?.Trim();

            m_RowKeys.RemoveAll(r => r.Data.ItemID == itemId);

            Predicate<LogRowData> validateFilter = rowData =>
            {
                if (String.IsNullOrEmpty(filter))
                    return true;

                if (this.comboBoxComparisonOperators.SelectedIndex > 0)
                {
                    // Comparison operator mode
                    if (TryParseDecimalInvariant(filter, out decimal constantValue))
                    {
                        return ((ValueComparison)this.comboBoxComparisonOperators.SelectedItem).Predicate(
                            rowData.Value,
                            constantValue);
                    }
                    return false; // We have a filter but it is no decimal value
                }

                // Normal text based filtering
                return rowData.Value.IndexOf(filter, StringComparison.OrdinalIgnoreCase) != -1;
            };

            for (int index = 0; index < rowUniqueIDs.Length; index++)
            {
                LogRowData logRowData = logRowDatas[index];

                // Filter values
                if (!validateFilter(logRowData))
                    continue;

                m_RowKeys.Add(new LogRowKey(rowUniqueIDs[index], logRowData));
            }

            m_RowKeys.Sort(this);

            m_List.VirtualListSize = m_RowKeys.Count;

            m_ItemCount[itemId] = count;

            //Scroll to last position
            GotoItem(item);
        }

        private void GotoItem([CanBeNull] ListViewItem item)
        {
            if (item == null)
                return;

            var key = (LogRowKey)item.Tag;
            ListViewItem lvi = m_List.FindItemWithText(key.Data.CycleIndex.ToString());

            if (lvi == null)
                return;

            int index = lvi.Index;
            int topIndex = Math.Max(0, (int)Math.Round(index - NumberOfVisibleItems * 0.5));
            m_List.TopItem = m_List.Items[topIndex];
            m_List.SelectedIndices.Add(index);
            m_List.Refresh();
        }

        private void RemoveItem(string itemId)
        {
            ListViewItem selectedItem = GetCurrentItem();
            Debug.WriteLine($"Remove item {itemId}");

            CheckedItems.Remove(itemId);

            // Check if item is in ItemReplacements.data
            string[] replacedIds = ((LogDatabase)Database).FindReplacedItem(itemId);
            foreach (string id in replacedIds)
            {
                m_RowKeys.RemoveAll(key => key.Data.ItemID == id);
                m_ItemCount.Remove(id);
            }

            m_RowKeys.RemoveAll(key => key.Data.ItemID == itemId);

            m_ItemCount.Remove(itemId);

            m_List.VirtualListSize = m_RowKeys.Count;

            //Scroll to last position
            GotoItem(selectedItem);
        }

        #region IComparer<LogRowKey> Members

        int IComparer<LogRowKey>.Compare(LogRowKey x, LogRowKey y)
        {
            if (m_SortOrder == SortOrder.Descending)
            {
                LogRowKey t = x;
                x = y;
                y = t;
            }

            switch (m_SortColumn)
            {
                case SortColumn.TimeStamp:
                    return x.UniqueId.CompareTo(y.UniqueId);
                case SortColumn.Name:
                    return string.Compare(x.Data.ItemID, y.Data.ItemID);
                case SortColumn.User:
                    return string.Compare(x.Data.User, y.Data.User);
                case SortColumn.Cycle:
                    return x.Data.CycleIndex.CompareTo(y.Data.CycleIndex);
                case SortColumn.Value:
                    return CompareValues(x.Data.Value, y.Data.Value);
                default:
                    return 0;
            }
        }

        private int CompareValues(string value1, string value2)
        {
            if (double.TryParse(value1, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                    m_Culture.NumberFormat, out double doubleValue1) && double.TryParse(value2,
                    NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, m_Culture.NumberFormat,
                    out double doubleValue2))
                return doubleValue1.CompareTo(doubleValue2);

            return value1.CompareTo(value2);
        }

        #endregion

        private string FormatItemKey(string itemID, string value)
        {
            return string.Format("{0}:{1}", itemID, value);
        }

        private bool ResolveItemColor(string itemId, string value, out Color color)
        {
            return m_ColorMap.TryGetValue(FormatItemKey(itemId, value), out color) ||  m_ColorMap.TryGetValue(itemId, out color);
        }


        private void m_List_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = CreateItemFromIndex(e.ItemIndex);

            string itemId = ((LogRowKey)e.Item.Tag).Data.ItemID;
            string value = ((LogRowKey)e.Item.Tag).Data.Value;

            if (ResolveItemColor(itemId, value, out Color color))
                e.Item.BackColor = color;
            else if (itemId == m_SelectedItem)
                e.Item.BackColor = Color.LightGray;
            else
                e.Item.BackColor = SystemColors.Window;
        }

        private void m_List_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ColumnHeader header = m_List.Columns[e.Column];

            var column = (SortColumn)header.DisplayIndex;

            if (m_SortColumn == column)
                m_SortOrder = m_SortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            else
                m_SortColumn = column;

            m_RowKeys.Sort(this);
            m_List.Invalidate();
        }

        private void m_List_DoubleClick(object sender, EventArgs e)
        {
            if (m_List.SelectedIndices.Count > 0)
            {
                LogRowKey key = m_RowKeys[m_List.SelectedIndices[0]];
                SelectedItem = key.Data.ItemID;
            }
        }

        private ListViewItem CreateItemFromIndex(int index)
        {
            LogRowKey key = m_RowKeys[index];

            LogRowData data = key.Data;

            var item = new ListViewItem(data.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            item.SubItems.Add(data.ItemID);
            item.SubItems.Add(data.Value);
            item.SubItems.Add(data.CycleIndex.ToString());
            item.SubItems.Add(data.User);
            item.Tag = key;
            return item;
        }

        #region IDataItemSubscriber Members

        private IItemDatabase m_Database;

        #endregion

        public void Print(Graphics g, RectangleF rect)
        {
            using (var bmp = new Bitmap((int)rect.Width, (int)rect.Height))
            {
                m_List.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                g.DrawImage(bmp, 0, 0);
            }
        }

        private readonly CultureInfo m_Culture;

        public void Initialize()
        {
            CheckedItems.ListChanged += OnCheckedItemsListChanged;
        }

        private void OnCheckedItemsListChanged(object sender, ListChangedEventArgs e)
        {
            UpdateReadingPane();
        }

        private void UpdateReadingPane()
        {
            foreach (string item in CheckedItems)
            {
                if (!m_ItemCount.ContainsKey(item))
                    AddItem(item);
            }

            string[] itemsToRemove = m_ItemCount.Keys
                .Where(item => !CheckedItems.Contains(item))
                .ToArray(); // This is important

            itemsToRemove.ForEach(RemoveItem);
        }

        public IItemDatabase Database
        {
            get { return m_Database; }
            set
            {
                m_List.VirtualListSize = 0;
                m_List.Items.Clear();
                m_CurrentState.Items.Clear();
                m_List.ClearHistory();
                m_CurrentState.ClearHistory();
                //ClearCachedItems();


                m_RowKeys.Clear();
                m_ItemCount.Clear();

                if (m_Database != null)
                {
                    m_Database.OnIndexesChanged -= OnIndexesChanged;
                    m_Database.OnIndexesAdded -= OnIndexAdded;
                    m_Database.OnInitialReadDone -= OnDatabaseInitialReadDone;
                }

                m_Database = value;


                if (m_Database != null)
                {
                    m_Database.OnIndexesChanged += OnIndexesChanged;
                    m_Database.OnIndexesAdded += OnIndexAdded;
                    m_Database.OnInitialReadDone += OnDatabaseInitialReadDone;
                   
                }

                m_TimingMeasurements.Database = m_Database;
            }
        }

        private void OnDatabaseInitialReadDone(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<EventArgs>(OnDatabaseInitialReadDone), sender, e);
                return;
            }            
            var itemIDs = m_Database.GetItemsIDs().ToList();
            
            CheckedItems.ListChanged -= OnCheckedItemsListChanged;
            
            CheckedItems
                .Where(x => !itemIDs.Contains(x))
                .ToList()
                .ForEach(RemoveItem);        
            
            CheckedItems.ListChanged += OnCheckedItemsListChanged;
        }

        private void OnIndexAdded(object sender, LogIndexAddedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<LogIndexAddedEventArgs>(OnIndexAdded), sender, e);
                return;
            }

            foreach (string item in e.AddedIndexes)
            {
                if (CheckedItems.Contains(item) && !m_ItemCount.ContainsKey(item))
                    AddItem(item);
            }
        }

        public string SelectedItem
        {
            get { return m_SelectedItem; }
            set
            {
                m_SelectedItem = value;
                m_List.Invalidate();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedItem"));
            }
        }

        public BindingListEx<string> CheckedItems { get; } = new BindingListEx<string>();

        public event PropertyChangedEventHandler PropertyChanged;

        private void m_List_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = e.NewWidth < 20;
        }

        private void m_List_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (m_List.Columns[e.ColumnIndex].Width < 20)
                m_List.Columns[e.ColumnIndex].Width = 20;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            m_Copy.Enabled = m_List.SelectedIndices.Count > 0;
            m_CycleReaderButton.Enabled = m_Database != null;

            m_ShowStateView.Enabled = true;
            m_SetMeasureStart.Enabled = m_List.VirtualListSize > 0;
            m_SetMeasureEnd.Enabled = m_List.VirtualListSize > 0;
        }

        private void m_Copy_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();

            var selectedIndexes = new int[m_List.SelectedIndices.Count];

            m_List.SelectedIndices.CopyTo(selectedIndexes, 0);

            foreach (int t in selectedIndexes)
            {
                if (m_List.Items[t].Tag is LogRowKey item)
                    sb.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}", item.Data.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                        item.Data.ItemID, item.Data.CycleIndex, item.Data.User, item.Data.Value);

                sb.AppendLine();
            }

            Clipboard.SetText(sb.ToString(), TextDataFormat.Text);
        }

        private void m_CycleReaderButton_Click(object sender, EventArgs e)
        {
            long start = 0;
            if (m_List.SelectedIndices.Count > 0)
            {
                start = ((LogRowKey)m_List.Items[m_List.SelectedIndices[0]].Tag).UniqueId;
            }

            using (var viewer = new CycleViewer(m_Database, start, m_ColorMap))
            {
                viewer.ShowDialog(this);
            }
        }

        private void editDetailsReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private char? GetNumericCharFromKeyEvent(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.D0:
                case Keys.NumPad0:
                    return '0';
                case Keys.D1:
                case Keys.NumPad1:
                    return '1';
                case Keys.D2:
                case Keys.NumPad2:
                    return '2';
                case Keys.D3:
                case Keys.NumPad3:
                    return '3';
                case Keys.D4:
                case Keys.NumPad4:
                    return '4';
                case Keys.D5:
                case Keys.NumPad5:
                    return '5';
                case Keys.D6:
                case Keys.NumPad6:
                    return '6';
                case Keys.D7:
                case Keys.NumPad7:
                    return '7';
                case Keys.D8:
                case Keys.NumPad8:
                    return '8';
                case Keys.D9:
                case Keys.NumPad9:
                    return '9';
            }

            return null;
        }

        private void m_List_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Right:
                    JumpToNextPost();
                    e.Handled = true;
                    return;
                case Keys.Left:
                    JumToPrevPost();
                    e.Handled = true;
                    return;
            }

            if (e.KeyCode == Keys.A && e.Control)
            {
                m_List.MultiSelect = true;
                int count = m_List.VirtualListSize;

                Cursor = Cursors.WaitCursor;
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        m_List.Items[i].Selected = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }

            char? numberAsChar = GetNumericCharFromKeyEvent(e);
            if(numberAsChar!=null)
            {
                e.SuppressKeyPress = ChangeRowColor(numberAsChar, e.Alt);
            }
        }

        private void JumpToRow(Func<IEnumerable<LogRowKey>, string, long, LogRowKey> findTargetRowFunc)
        {
            ListViewItem item = m_List.FocusedItem;

            if (item != null)
            {
                var rowKey = (LogRowKey)item.Tag;

                LogRowKey nextKey = findTargetRowFunc(m_RowKeys, rowKey.Data.ItemID, rowKey.UniqueId);

                SelectListItemFromRowKey(nextKey);
            }
        }

        private void SelectListItemFromRowKey([CanBeNull] LogRowKey nextKey)
        {
            if (nextKey == null)
                return;

            if (!CheckedItems.Contains(nextKey.Data.ItemID))
            {
                CheckedItems.Add(nextKey.Data.ItemID);
            }

            int nextIndex = m_RowKeys.IndexOf(nextKey);

            ListViewItem nextItem = m_List.Items[nextIndex];

            m_List.SelectedIndices.Clear();
            nextItem.Focused = true;
            nextItem.Selected = true;
            nextItem.EnsureVisible();
        }

        private void JumpToNextPost()
        {
            JumpToRow((keys, itemId, uniqueId) =>
                keys.FirstOrDefault(key => key.Data.ItemID.Equals(itemId) && key.UniqueId > uniqueId));
        }

        private void JumToPrevPost()
        {
            JumpToRow((keys, itemId, uniqueId) =>
                keys.LastOrDefault(key => key.Data.ItemID.Equals(itemId) && key.UniqueId < uniqueId));
        }

        private void m_List_SearchForVirtualItem(object sender, SearchForVirtualItemEventArgs e)
        {
            int.TryParse(e.Text, out int cycleIndex);
            if (cycleIndex == 0)
                return;

            int index = m_RowKeys.FindIndex(p => p.Data.CycleIndex >= cycleIndex);
            e.Index = index;
        }


        private bool ChangeRowColor(char? c, bool filterOnValue)
        {
            ListViewItem item = m_List.FocusedItem;
            if (item == null)
                return false;

            string itemId = ((LogRowKey)item.Tag).Data.ItemID;
            string value = ((LogRowKey)item.Tag).Data.Value;

            string keyWithValue = FormatItemKey(itemId, value);
            string key = filterOnValue ? keyWithValue : itemId;

            switch (c)
            {
                case '0':
                    if (m_ColorMap.ContainsKey(keyWithValue))
                    {
                        m_ColorMap.Remove(keyWithValue);
                    }
                    else
                    {
                        m_ColorMap.Remove(key);
                    }
                    break;
                case '1':
                    m_ColorMap[key] = Color.FromArgb(73, 171, 214);
                    break;
                case '2':
                    m_ColorMap[key] = Color.FromArgb(112, 193, 179);
                    break;
                case '3':
                    m_ColorMap[key] = Color.FromArgb(178, 219, 191);
                    break;
                case '4':
                    m_ColorMap[key] = Color.FromArgb(243, 255, 189);
                    break;
                case '5':
                    m_ColorMap[key] = Color.FromArgb(255, 22, 84);
                    break;
                case '6':
                    m_ColorMap[key] = Color.FromArgb(0, 240, 180);
                    break;
                case '7':
                    m_ColorMap[key] = Color.FromArgb(185, 255, 185);
                    break;
                case '8':
                    m_ColorMap[key] = Color.FromArgb(128, 255, 255);
                    break;
                case '9':
                    m_ColorMap[key] = Color.FromArgb(216, 167, 213);
                    break;
                default:
                    return false;
            }

            m_List.Refresh();

            return true;
        }

        private void m_List_SelectedItemChanged(object sender, EventArgs e)
        {
            //Don't update State list if the lower panel is collapsed
            if (!m_SplitContainer.Panel2Collapsed)
                UpdateStateList();
        }

        private void UpdateStateList()
        {
            ListViewItem focusedItem = m_List.FocusedItem;

            if (focusedItem != null)
            {
                var rowKey = (LogRowKey)focusedItem.Tag;

                Change change = ChangeFactory.CreateFrom(m_Database, rowKey, 10);

                UpdateGraph(change);

                IEnumerable<IGrouping<string, LogRowKey>> items = m_RowKeys.GroupBy(key => key.Data.ItemID);

                IEnumerable<LogRowKey> lastValueFromEachKey = items.Select(keys => keys.LastOrDefault(key =>
                        key.UniqueId <= rowKey.UniqueId || key.Data.CycleIndex == rowKey.Data.CycleIndex))
                    .Where(k => k != null);


                m_CurrentState.BeginUpdate();
                {
                    m_CurrentState.Items.Clear();

                    foreach (LogRowKey item in lastValueFromEachKey.OrderBy(key => key.UniqueId))
                    {
                        var listItem = new ListViewItem(item.Data.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff"))
                        {
                            Tag = item,
                            Group = m_CurrentState.Groups["currentStateGroup"]
                        };
                        listItem.SubItems.AddRange(new[]
                            {item.Data.ItemID, item.Data.Value, item.Data.CycleIndex.ToString(), item.Data.User});

                        listItem.ForeColor = item.Data.CycleIndex == rowKey.Data.CycleIndex
                            ? DefaultForeColor
                            : Color.Gray;
                        m_CurrentState.Items.Add(listItem);
                    }

                    foreach (LogRowKey trigger in change.States.OrderBy(trigger => trigger.Key.UniqueId)
                        .Select(p => p.Key))
                    {
                        var listItem = new ListViewItem(trigger.Data.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff"))
                        {
                            Tag = trigger,
                            Group = m_CurrentState.Groups["triggersGroup"]
                        };
                        listItem.SubItems.AddRange(new[]
                        {
                            trigger.Data.ItemID, trigger.Data.Value, trigger.Data.CycleIndex.ToString(),
                            trigger.Data.User
                        });

                        //listItem.ForeColor = (trigger.Data.CycleIndex == rowKey.Data.CycleIndex) ? DefaultForeColor : Color.Gray;
                        m_CurrentState.Items.Add(listItem);
                    }
                }
                m_CurrentState.EndUpdate();
            }
            else
                m_CurrentState.Items.Clear();
        }

        private void toolStripMenuItem2_CheckedChanged(object sender, EventArgs e)
        {
            m_SplitContainer.Panel2Collapsed = !m_ShowStateView.Checked;
            if (!m_SplitContainer.Panel2Collapsed) 
                UpdateStateList();
        }

        private void m_CurrentState_DoubleClick(object sender, EventArgs e)
        {
            Point screenPoint = Cursor.Position;
            Point controlPoint = m_CurrentState.PointToClient(screenPoint);
            ListViewItem item = m_CurrentState.GetItemAt(controlPoint.X, controlPoint.Y);

            if (item != null)
            {
                var key = item.Tag as LogRowKey;

                SelectListItemFromRowKey(key);
                m_List.Focus();
            }
        }

        private void UpdateGraph(Change change)
        {
            var graph = new Graph("graph");

            UpdateGraph(graph, change, true);

            Node node = graph.AddNode("   ");
            graph.AddEdge(change.Key.Data.ItemID, change.Key.Data.Value, node.Id);

            CleanUpEdges(graph);
            m_Viewer.Graph = graph;
        }

        private static void CleanUpEdges(Graph graph)
        {
            ILookup<string, Edge> lookup = graph.Edges.ToLookup(edge => edge.Source + edge.Target);

            foreach (IGrouping<string, Edge> group in lookup.Where(p => p.Count() > 1))
            {
                foreach (Edge edge in group.Skip(1))
                {
                    graph.RemoveEdge(edge);
                }
            }
        }

        private static bool TryParseDecimalInvariant(string value, out decimal decimalValue)
        {
            return Decimal.TryParse(
                value,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out decimalValue);
        }

        private static void UpdateGraph(Graph graph, Change change, bool active)
        {
            Node node = graph.FindNode(change.Key.Data.ItemID);
            if (node == null)
            {
                node = graph.AddNode(change.Key.Data.ItemID);
                node.UserData = change;
            }

            bool hasCauses = false;
            foreach ((Change, bool) changeCause in change.GetStatesAndCause())
            {
                hasCauses = true;
                UpdateGraph(graph, changeCause.Item1, changeCause.Item2 && active);

                string edgeLabel = changeCause.Item2 && active ? changeCause.Item1.Key.Data.Value : string.Empty;
                Edge edge = graph.AddEdge(changeCause.Item1.Key.Data.ItemID, edgeLabel, change.Key.Data.ItemID);
                edge.UserData = changeCause.Item1;
                edge.Attr.Color = changeCause.Item2 && active
                    ? Microsoft.Msagl.Drawing.Color.Blue
                    : Microsoft.Msagl.Drawing.Color.Gray;
                edge.Attr.Weight = changeCause.Item2 && active ? 2 : 1;
            }

            if (!hasCauses)
            {
                Node userNode = graph.FindNode(change.Key.Data.User);
                if (userNode == null)
                {
                    userNode = graph.AddNode(change.Key.Data.User);
                    userNode.UserData = change;
                }

                string edgeLabel = active ? change.Key.Data.Value : string.Empty;
                Edge edge = graph.AddEdge(change.Key.Data.User, edgeLabel, change.Key.Data.ItemID);
                edge.UserData = change;
                edge.Attr.Color = active ? Microsoft.Msagl.Drawing.Color.Blue : Microsoft.Msagl.Drawing.Color.Gray;
                edge.Attr.Weight = active ? 2 : 1;
            }
        }

        public void GotoTime(string itemId, DateTime timestamp)
        {
            LogRowKey rowKey = m_RowKeys.Find(key =>
                key.Data.ItemID == itemId && key.Data.TimeStamp.Ticks == timestamp.Ticks);

            if (rowKey != null)
                SelectListItemFromRowKey(rowKey);

            Focus();
        }

        private void SetMeasureStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_List.SelectedIndices.Count > 0)
            {
                int index = m_List.SelectedIndices[0];

                LogRowKey measureStart = m_RowKeys[index];
                m_TimingMeasurements.SetStart(measureStart.Data.ItemID, measureStart.Data.Value);

                m_TabControl.SelectedTab = m_Measurements;
                m_ShowStateView.Checked = true;
            }
        }

        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (m_List.SelectedIndices.Count > 0)
            {
                int index = m_List.SelectedIndices[0];
                LogRowKey measureEnd = m_RowKeys[index];
                m_TimingMeasurements.SetEnd(measureEnd.Data.ItemID, measureEnd.Data.Value);
                m_TimingMeasurements.Calculate();
            }
        }

        private void TextBoxFilter_TextChanged(object sender, EventArgs e)
        {
            m_ItemCount.Clear();
            UpdateReadingPane();
        }

        private void ButtonResetFilter_Click(object sender, EventArgs e)
        {
            this.textBoxFilter.Text = null;
        }

        private void ComboBoxComparisonOperators_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_ItemCount.Clear();
            UpdateReadingPane();
        }
    }

    public class LogRowKey
    {
        public readonly long UniqueId;
        public readonly LogRowData Data;

        public LogRowKey(long uniqueId, LogRowData data)
        {
            UniqueId = uniqueId;
            Data = data;
        }

        public override bool Equals(object obj)
        {
            if (obj is LogRowKey log)
            {
                return UniqueId == log.UniqueId;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return UniqueId.GetHashCode();
        }
    }
}