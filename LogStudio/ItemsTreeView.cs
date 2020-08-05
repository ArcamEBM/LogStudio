using JetBrains.Annotations;
using LogStudio.Data;
using LogStudio.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LogStudio
{
    public partial class ItemsTreeView : UserControl, IComparer
    {
        private readonly Bitmap m_Collapsed;
        private readonly Bitmap m_Expanded;
        private readonly Bitmap m_SelectedFolder;
        private readonly Bitmap m_Folder;
        private readonly List<string> m_ItemsToCheck = new List<string>();
        private readonly List<string> m_Items = new List<string>();
        private readonly Dictionary<string, TreeNode> m_NodeByItemID = new Dictionary<string, TreeNode>();
        private readonly SolidBrush m_Background = new SolidBrush(SystemColors.Window);
        private readonly SolidBrush m_TextBackground = new SolidBrush(SystemColors.Highlight);

        public ItemsTreeView()
        {
            InitializeComponent();
            m_Collapsed = Properties.Resources.Collapsed;
            m_Collapsed.MakeTransparent();

            m_Expanded = Properties.Resources.Expanded;
            m_Expanded.MakeTransparent();

            m_Folder = Properties.Resources.VSFolder_closed;
            m_Folder.MakeTransparent();

            m_SelectedFolder = Properties.Resources.VSFolder_open;
            m_SelectedFolder.MakeTransparent();

            m_Tree.TreeViewNodeSorter = this;
        }

        private IItemDatabase m_Database;

        [PublicAPI] // get needed for designer
        public IItemDatabase ItemDatabase
        {
            get => m_Database;
            set
            {
                if (m_Database != null)
                {
                    m_Database.OnIndexesAdded -= OnIndexesAdded;
                    m_Database.OnIndexesChanged -= OnIndexChanged;
                    m_Database.OnInitialReadDone -= OnInitialReadDone;
                }

                m_Database = value;

                m_Items.Clear();
                m_Tree.Nodes.Clear();
                m_NodeByItemID.Clear();

                if (m_Database == null)
                    return;

                if (m_Module?.CheckedItems != null)
                    m_ItemsToCheck.AddRange(m_Module.CheckedItems);
                
                m_Database.OnIndexesAdded += OnIndexesAdded;
                m_Database.OnIndexesChanged += OnIndexChanged;
                m_Database.OnInitialReadDone += OnInitialReadDone;

                BuildTree(m_Database.GetItemsIDs());
            }
        }

        private void OnInitialReadDone(object sender, EventArgs e)
        {
            var itemIDs= m_Database.GetItemsIDs().ToList();
            m_ItemsToCheck
                .Where(x => !itemIDs.Contains(x))
                .ToList()
                .ForEach(x => m_ItemsToCheck.Remove(x));
        }

        private void OnIndexChanged(object sender, LogIndexChangesEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<LogIndexChangesEventArgs>(OnIndexChanged), sender, e);
                return;
            }

            foreach (LogIndexChangeRange range in e.Changes)
            {
                try
                {
                    var leaf = (TreeNodeLeaf)m_NodeByItemID[range.ItemID];
                    leaf.UpdateItemCount((TreeNodeFolder)leaf.Parent, range.Index + range.Count);
                }
                catch (KeyNotFoundException)
                {
                }
            }
        }

        private IModule m_Module;

        [PublicAPI] // get needed for designer
        public IModule Module
        {
            get => m_Module;
            set
            {
                if (m_Module != null)
                {
                    m_Module.PropertyChanged -= OnModulePropertyChanged;
                    if (m_Module.CheckedItems != null)
                    {
                        m_Module.CheckedItems.ListChanged -= OnCheckedItemsChanged;
                        m_Module.CheckedItems.OnItemRemoved += CheckedItemsOnOnItemRemoved;
                    }

                    m_Module.SelectedItem = string.Empty;
                }

                m_Module = null;

                m_Tree.SelectedNode = null;

                m_Tree.CollapseAll();
                UnCheckAllNodes(m_Tree.Nodes);

                m_Module = value;

                if (m_Module != null && m_Module.TreeVisible)
                {
                    m_Module.PropertyChanged += OnModulePropertyChanged;
                    if (m_Module.CheckedItems != null)
                    {
                        m_Module.CheckedItems.ListChanged += OnCheckedItemsChanged;
                        m_Module.CheckedItems.OnItemRemoved += CheckedItemsOnOnItemRemoved;

                        m_Module.CheckedItems.ResetBindings();
                    }
                }

                Enabled = m_Module?.CheckedItems != null;
            }
        }

        private void CheckedItemsOnOnItemRemoved(object sender, ItemRemovedEventArgs<string> e)
        {
            if (!m_NodeByItemID.TryGetValue(e.Item, out TreeNode node))
                return;

            node.Checked = false;
            if (node is TreeNodeLeaf leaf)
            {
                leaf.UnCheckLeaf();
            }
        }

        private void OnCheckedItemsChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    {
                        string itemId = m_Module.CheckedItems[e.NewIndex];
                        if (m_NodeByItemID.TryGetValue(itemId, out TreeNode node))
                        {
                            node.Checked = true;
                            node.EnsureVisible();

                            if (node is TreeNodeLeaf leaf)
                            {
                                leaf.CheckLeaf();
                                m_Tree.ExpandeNodePath(leaf);
                                leaf.EnsureVisible();
                            }
                        }
                    }
                    break;
                case ListChangedType.Reset:
                    {
                        UnCheckAllNodes(m_Tree.Nodes);

                        foreach (string itemId in m_Module.CheckedItems)
                        {
                            if (!m_NodeByItemID.TryGetValue(itemId, out TreeNode node))
                                continue;

                            node.Checked = true;
                            node.EnsureVisible();

                            if (!(node is TreeNodeLeaf leaf))
                                continue;

                            leaf.CheckLeaf();
                            m_Tree.ExpandeNodePath(leaf);
                            leaf.EnsureVisible();
                        }
                    }
                    break;
            }

            m_Tree.Invalidate();
        }

        private void OnModulePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedItem":
                    SelectItem(m_Module.SelectedItem);
                    break;
                case "CheckedItems":
                    {
                        Enabled = m_Module.CheckedItems != null;
                        m_Tree.CollapseAll();

                        if (m_Module.CheckedItems != null)
                        {
                            m_Module.CheckedItems.ListChanged += OnCheckedItemsChanged;
                            m_Module.CheckedItems.ResetBindings();
                        }
                        else
                        {
                            UnCheckAllNodes(m_Tree.Nodes);
                        }
                    }
                    break;
            }
        }

        private void OnIndexesAdded(object sender, LogIndexAddedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<LogIndexAddedEventArgs>(OnIndexesAdded), sender, e);
                return;
            }

            m_Items.AddRange(e.AddedIndexes);

            BuildTree(e.AddedIndexes);
        }

        private void BuildTree(IEnumerable<string> items)
        {
            //Dynamically build the tree
            foreach (string itemID in items)
            {
                TreeNodeCollection nodes = m_Tree.Nodes;
                TreeNodeFolder folder = null;

                string[] parts = itemID.Split('.');

                for (int i = 0; i < parts.Length; i++)
                {
                    string part = parts[i];


                    TreeNode node = nodes[part];

                    if (node == null)
                    {
                        bool check = false;
                        //Check if node is the leaf
                        if (i == parts.Length - 1)
                        {
                            node = new TreeNodeLeaf(folder, part, itemID, m_Database.GetItemRowCount(itemID))
                            {
                                ImageIndex = 2,
                                SelectedImageIndex = 2
                            };
                            if (m_ItemsToCheck.Contains(itemID))
                            {
                                check = true;
                                m_ItemsToCheck.Remove(itemID);
                            }

                            m_NodeByItemID.Add(itemID, node);
                        }
                        else
                        {
                            node = new TreeNodeFolder(part) { ImageIndex = 0, SelectedImageIndex = 1 };
                            folder = (TreeNodeFolder)node;
                        }

                        nodes.Add(node);

                        if (check)
                        {
                            node.Checked = true;
                            node.EnsureVisible();
                        }
                    }
                    else
                    {
                        folder = (TreeNodeFolder)node;
                    }


                    nodes = node.Nodes;
                }
            }
        }

        private void m_Tree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown)
                return;

            if (e.Node is TreeNodeLeaf leaf)
            {
                if (m_Module?.CheckedItems != null)
                {
                    if (leaf.Checked && !m_Module.CheckedItems.Contains(leaf.ItemID))
                        m_Module.CheckedItems.Add(leaf.ItemID);
                    else if (!leaf.Checked && m_Module.CheckedItems.Contains(leaf.ItemID))
                        m_Module.CheckedItems.Remove(leaf.ItemID);
                }
            }

            m_Tree.Invalidate();
        }

        private bool m_IgnoreSelectItem;

        private void m_Tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (m_Module == null)
                    return;

                m_IgnoreSelectItem = true;

                string itemID = string.Empty;

                if (e.Node is TreeNodeLeaf leaf)
                    itemID = leaf.ItemID;

                m_Module.SelectedItem = itemID;
                m_IgnoreSelectItem = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName);
            }
        }

        private void SelectItem(string itemID)
        {
            if (m_IgnoreSelectItem)
                return;

            if (string.IsNullOrEmpty(itemID))
            {
                m_Tree.SelectedNode = null;
                return;
            }

            try
            {
                m_Tree.SelectedNode = m_NodeByItemID[itemID];
            }
            catch
            {
                //item is not found in tree
                //because a filter was used to hide tree node
                throw;
            }
        }

        public string[] GetCheckedItems()
        {
            List<string> items = new List<string>();
            GetCheckedItems(m_Tree.Nodes, items);
            return items.ToArray();
        }

        private static void GetCheckedItems(TreeNodeCollection nodes, List<string> items)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Checked && node is TreeNodeLeaf leaf)
                    items.Add(leaf.ItemID);

                GetCheckedItems(node.Nodes, items);
            }
        }

        private static void UnCheckAllNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Checked)
                    node.Checked = false;

                if (node is TreeNodeLeaf leaf)
                    leaf.UnCheckLeaf();

                UnCheckAllNodes(node.Nodes);
            }
        }

        private void m_Tree_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            Font font = m_Tree.Font;

            if (e.Node is TreeNodeFolder folderNode)
            {
                Rectangle rect = e.Bounds;
                rect.X = 0;
                rect.Width = ClientRectangle.Width;
                e.Graphics.FillRectangle(m_Background, rect);

                TreeNodeStates state = e.State;
                Color backColor, foreColor;

                bool grayed = !m_Tree.Enabled || (state & TreeNodeStates.Grayed) == TreeNodeStates.Grayed;

                if (grayed)
                {
                    backColor = m_Tree.BackColor;
                    foreColor = SystemColors.GrayText;
                }
                else if ((state & TreeNodeStates.Marked) == TreeNodeStates.Marked)
                {
                    backColor = SystemColors.Highlight;
                    foreColor = SystemColors.HighlightText;
                }
                else if ((state & TreeNodeStates.Selected) == TreeNodeStates.Selected)
                {
                    backColor = SystemColors.Highlight;
                    foreColor = SystemColors.HighlightText;
                }
                else if ((state & TreeNodeStates.Hot) == TreeNodeStates.Hot)
                {
                    backColor = SystemColors.HotTrack;
                    foreColor = SystemColors.HighlightText;
                }
                else
                {
                    backColor = e.Node.BackColor;
                    foreColor = e.Node.ForeColor;
                }

                Rectangle newBounds = e.Node.Bounds;

                Point expandImagePos = newBounds.Location;
                expandImagePos.Offset(-34, 0);
                Point imagePos = newBounds.Location;
                imagePos.Offset(-17, 0);
                Point overlayPos = imagePos;
                overlayPos.Offset(-1, 7);

                Rectangle textBounds = newBounds;

                DrawTreeImage(
                    e.Graphics,
                    e.Node.IsExpanded ? m_Expanded : m_Collapsed,
                    expandImagePos,
                    grayed);

                DrawTreeImage(
                    e.Graphics,
                    m_Tree.SelectedNode == e.Node ? m_SelectedFolder : m_Folder,
                    imagePos,
                    grayed);

                if (folderNode.CheckedLeafesBelow > 0)
                {
                    e.Graphics.DrawImageUnscaled(Properties.Resources.checkedOverLay, overlayPos);
                }

                m_TextBackground.Color = backColor;
                e.Graphics.FillRectangle(m_TextBackground, textBounds);

                TextRenderer.DrawText(e.Graphics, e.Node.Text, font, textBounds, foreColor, backColor);

                e.DrawDefault = false;
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        private void DrawTreeImage(Graphics g, Image image, Point position, bool grayed)
        {
            if (!grayed)
                g.DrawImageUnscaled(image, position);
            else
                ControlPaint.DrawImageDisabled(g, image, position.X, position.Y, m_Tree.BackColor);
        }

        private void m_Tree_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void m_Tree_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                TreeNode node = m_Tree.GetNodeAt(e.Location);
                if (node == null)
                    return;

                m_Tree.SelectedNode = node;
                if (node.Nodes.Count == 0)
                    return;

                Rectangle hitArea = node.Bounds;
                hitArea.X -= 34;
                hitArea.Width = 16;

                if (hitArea.Contains(e.Location))
                    node.Toggle();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName);
            }
        }

        #region IComparer<TreeNode> Members

        int IComparer.Compare(object x, object y)
        {
            var xNode = (TreeNode)x;
            var yNode = (TreeNode)y;

            switch (xNode)
            {
                case TreeNodeFolder _ when yNode is TreeNodeLeaf:
                    return -1;
                case TreeNodeLeaf _ when yNode is TreeNodeFolder:
                    return 1;
                default:
                    return xNode.Text.CompareTo(yNode.Text);
            }
        }

        #endregion

        private void OnShowItemInformation(object sender, EventArgs e)
        {
            if (!(m_Tree.SelectedNode is TreeNodeLeaf leaf))
                return;

            try
            {
                using (var form = new ItemInformationForm(m_Database, leaf.ItemID))
                {
                    form.UpdateValues();
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName);
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            m_DeselectAllSubitems.Enabled = m_Tree.SelectedNode is TreeNodeFolder;
            m_ExpandAllSubnodes.Enabled = m_Tree.SelectedNode is TreeNodeFolder;
            m_CollapsAllSubnodes.Enabled = m_Tree.SelectedNode is TreeNodeFolder;
            m_ItemToClipboard.Enabled = m_Database != null && m_Tree.SelectedNode is TreeNodeLeaf;
            m_ShowItemInformation.Enabled = m_Database != null && m_Tree.SelectedNode is TreeNodeLeaf;
            m_ExportToCSV.Enabled = m_Database != null && m_Tree.SelectedNode is TreeNodeLeaf;
            m_ExportStatesToCSV.Enabled = m_Database != null && m_Tree.SelectedNode is TreeNodeLeaf;
        }

        private void OnExportToCSV(object sender, EventArgs e)
        {
            if (m_DlgExportToCSV.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                m_Database.ExportToCSV(m_DlgExportToCSV.FileName, ((TreeNodeLeaf)m_Tree.SelectedNode).ItemID, ',');
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to export to CSV!");
            }
        }

        private void itemNameToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(((TreeNodeLeaf)m_Tree.SelectedNode).ItemID);
        }

        private void OnSearch(object sender, EventArgs e)
        {
            UpdateTree();
        }

        private void UpdateTree()
        {
            string filterText = m_FilterText.Text.Trim();

            m_Tree.BeginUpdate();
            try
            {
                m_Tree.Nodes.Clear();
                m_NodeByItemID.Clear();

                BuildTree(m_Items.Where(
                    (item) => item.IndexOf(filterText, StringComparison.CurrentCultureIgnoreCase) >= 0));

                m_Module.CheckedItems.ResetBindings();

                if (m_FilterText.Text.Length > 0)
                {
                    m_RemoveSearch.Enabled = true;
                    m_Tree.ExpandAll();
                }
                else
                    m_RemoveSearch.Enabled = false;
            }
            finally
            {
                m_Tree.EndUpdate();
            }
        }

        private void m_FilterText_TextChanged(object sender, EventArgs e)
        {
        }

        private void m_FilterText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                UpdateTree();
        }

        private void m_CollapsAllSubnodes_Click(object sender, EventArgs e)
        {
            m_Tree.SelectedNode?.Collapse(false);
        }

        private void OnExpandAllSubnodes(object sender, EventArgs e)
        {
            m_Tree.SelectedNode?.ExpandAll();
        }

        private void OnDeselectAllSubitems(object sender, EventArgs e)
        {
            m_Module.CheckedItems.RaiseListChangedEvents = false;

            if (m_Tree.SelectedNode is TreeNodeFolder folder)
            {
                foreach (string itemId in folder.GetChildrenIds())
                {
                    m_Module.CheckedItems.Remove(itemId);
                }
            }

            m_Module.CheckedItems.RaiseListChangedEvents = true;
            m_Module.CheckedItems.ResetBindings();

            m_Tree.Invalidate();
        }

        private void OnRemoveSearch(object sender, EventArgs e)
        {
            m_FilterText.Text = "";
            UpdateTree();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            m_Module.CheckedItems.RaiseListChangedEvents = false;

            if (m_Tree.SelectedNode is TreeNodeFolder folder)
            {
                foreach (string itemId in folder.GetChildrenIds())
                {
                    if (!m_Module.CheckedItems.Contains(itemId))
                        m_Module.CheckedItems.Add(itemId);
                }
            }

            m_Module.CheckedItems.RaiseListChangedEvents = true;
            m_Module.CheckedItems.ResetBindings();

            m_Tree.Invalidate();
        }

        private void m_ExportStatesToCSV_Click(object sender, EventArgs e)
        {
            if (m_DlgExportStatesToCSV.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                m_Database.ExportStateOnItemChangeToCSV(m_DlgExportStatesToCSV.FileName,
                    ((TreeNodeLeaf)m_Tree.SelectedNode).ItemID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to export item states to CSV!");
            }
        }

        private void m_Tree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null)
                return;
            if (!(e.Node is TreeNodeLeaf leaf))
                return;

            LogRowData data = m_Database.GetItemRow(leaf.ItemID, 0);
            m_Module.GotoTime(leaf.ItemID, data.TimeStamp);
        }
    }

    [Serializable]
    public class TreeNodeFolder : TreeNode
    {
        public TreeNodeFolder(string text)
            : base(text)
        {
            Name = text;
            ForeColor = Color.FromArgb(120, 120, 120);
        }

        public int CheckedLeafesBelow { get; private set; }

        private bool ContainsChangedItems { get; set; }

        public void IncCheckedItemsBelow(int value)
        {
            CheckedLeafesBelow += value;

            if (!(Parent is TreeNodeFolder node))
                return;

            node.IncCheckedItemsBelow(value);
            if (node.IsVisible)
                node.TreeView.Invalidate(node.Bounds);
        }

        public void UpdateContainsChangedItems()
        {
            if (ContainsChangedItems)
                return;

            ContainsChangedItems = true;

            ForeColor = Color.Black;

            if (Parent is TreeNodeFolder parent)
                parent.UpdateContainsChangedItems();
        }

        public IEnumerable<string> GetChildrenIds()
        {
            foreach (object node in Nodes)
            {
                switch (node)
                {
                    case TreeNodeFolder folder:
                        {
                            foreach (string childId in folder.GetChildrenIds())
                            {
                                yield return childId;
                            }

                            break;
                        }
                    case TreeNodeLeaf leaf:
                        yield return leaf.ItemID;
                        break;
                }
            }
        }

        protected TreeNodeFolder(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class TreeNodeLeaf : TreeNode
    {
        public TreeNodeLeaf(TreeNodeFolder parent, string text, string itemID, int count)
            : base(text)
        {
            ItemID = itemID;
            Name = text;

            UpdateItemCount(parent, count);
        }

        public readonly string ItemID;

        public void CheckLeaf()
        {
            if (Parent is TreeNodeFolder node)
            {
                node.IncCheckedItemsBelow(1);
            }
        }

        public void UnCheckLeaf()
        {
            if (Parent is TreeNodeFolder node)
            {
                node.IncCheckedItemsBelow(-1);
            }
        }

        public void UpdateItemCount(TreeNodeFolder parent, int count)
        {
            ForeColor = count > 1 ? Color.Black : Color.FromArgb(120, 120, 120);

            if (parent != null && count > 1)
                parent.UpdateContainsChangedItems();
        }

        protected TreeNodeLeaf(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
}