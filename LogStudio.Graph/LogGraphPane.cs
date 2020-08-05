using LogStudio.Data;
using LogStudio.Framework;
using LogStudio.Graph.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using ZedGraph;

namespace LogStudio
{
    public class LogGraphPane : GraphPane
    {
        public event EventHandler OnCurveItemSelected;

        private ColorSymbolRotator m_Rotator = new ColorSymbolRotator();

        private const int MaxEntriesInCurveToPopulateGridItems = 2000;

        private LogGraphPage m_Parent;
        private LineObj m_Threshold;
        private object m_SelectedObject = null;
        private Color m_SelectedObjectOriginalColor;
        private int m_SelectedIndex = 0;

        private string m_MappedItem = string.Empty;
        private IModuleManager m_Manager;
        private PointF m_RightClickPosition;
        public LogGraphPane(LogGraphPage parent, IModuleManager manager)
        {
            m_Parent = parent;
            m_Manager = manager;
            CheckedItems = new BindingListEx<string>();
            CheckedItems.ListChanged += new ListChangedEventHandler(OnCheckedItemsChanged);
            Title.IsVisible = false;

            base.XAxis.Type = AxisType.Date;
            base.XAxis.Title.Text = "Time";
            base.XAxis.Scale.FontSpec.Family = "Verdana";
            base.XAxis.Scale.FontSpec.Size = 10;
            //            base.XAxis.Grid = true;

            base.XAxis.Title.FontSpec.Family = "Verdana";
            base.XAxis.Title.FontSpec.Size = 12;
            base.XAxis.Title.FontSpec.IsBold = false;


            base.Legend.FontSpec.Family = "Verdana";
            base.Legend.FontSpec.Size = 10;


            base.YAxis.Title.Text = "Value";

            base.YAxis.Scale.FontSpec.Family = "Verdana";
            base.YAxis.Scale.FontSpec.Size = 10;

            base.YAxis.Title.FontSpec.Family = "Verdana";
            base.YAxis.Title.FontSpec.Size = 12;
            base.YAxis.Title.FontSpec.IsBold = false;
            //base.YAxis.IsShowGrid = true;

            base.Y2Axis.Title.Text = "Value";
            base.Y2Axis.IsVisible = false;
            base.Y2Axis.Title.FontSpec.Family = "Verdana";
            base.Y2Axis.Title.FontSpec.Size = 12;
            base.Y2Axis.Title.FontSpec.IsBold = false;
            base.Y2Axis.Scale.FontSpec.Family = "Verdana";
            base.Y2Axis.Scale.FontSpec.Size = 10;
            //base.Y2Axis.StepAuto = true;
            //base.Y2Axis.ScaleFormatAuto = true;
            //base.Y2Axis.ScaleMagAuto = true;

            base.IsFontsScaled = false;
            AxisChange();
        }

        public LogGraphPage Parent
        {
            get
            {
                return m_Parent;
            }
        }

        void OnCheckedItemsChanged(object sender, ListChangedEventArgs e)
        {
            foreach (string item in CheckedItems)
            {
                if (CurveList.IndexOfTag(item) < 0)
                    AddItem(item);
            }

            List<string> itemsToRemove = new List<string>();
            foreach (LogGraphCurveItem item in CurveList)
            {
                if (!CheckedItems.Contains(item.ItemID))
                    itemsToRemove.Add(item.ItemID);
            }

            foreach (string item in itemsToRemove)
            {
                RemoveItem(item);
            }
        }

        public BindingListEx<string> CheckedItems { get; private set; }

        public void AddItem(string item)
        {
            Color color = m_Rotator.NextColor;
            if (color == Color.Red)
                color = m_Rotator.NextColor;
            AddCuveItem(item, color);
            CheckItemRenderingCapability(item);
        }

        public void SetThreshold(double? value)
        {
            if (value.HasValue)
            {
                if (m_Threshold == null)
                {
                    m_Threshold = new LineObj(Color.Red, 0, value.Value, 1, value.Value)
                    {
                        Location = { CoordinateFrame = CoordType.XChartFractionYScale },
                        IsClippedToChartRect = true
                    };
                    GraphObjList.Add(m_Threshold);
                }
                else
                {
                    m_Threshold.Location.Y = value.Value;
                }
            }
            else
            {
                GraphObjList.Remove(m_Threshold);
                m_Threshold = null;
            }

            m_Parent.UpdateGraph();
        }

        private LogGraphCurveItem AddCuveItem(string item, Color color)
        {
            LogGraphCurveItem line = new LogGraphCurveItem(item, this, this.Parent);
            line.Database = m_ItemDatabase;
            line.Color = color;
            line.Symbol.Type = m_Rotator.NextSymbol;
            line.Tag = item;
            if (IsBooleanItem(item))
            {
                line.Label.Text += " (Y2)";
                line.IsY2Axis = true;
                Y2Axis.IsVisible = true;
            }
            CurveList.Add(line);

            m_Parent.ResetAutoScale(this);

            m_Parent.UpdateGraph();

            m_HasChanged = true;

            return line;
        }


        public void RemoveItem(string item)
        {
            //Remove the item counter
            CheckedItems.RaiseListChangedEvents = false;
            CheckedItems.Remove(item);
            CheckedItems.RaiseListChangedEvents = true;

            CurveList.RemoveAll(
                new Predicate<CurveItem>(
                    delegate (CurveItem line)
                    {
                        return ((LogGraphCurveItem)line).ItemID == item;
                    }));

            AxisChange();
            m_Parent.UpdateGraph();

            if (SelectedItem == item && OnCurveItemSelected != null)
                OnCurveItemSelected(this, EventArgs.Empty);

            m_HasChanged = true;
        }

        public string[] SubscribedItems
        {
            get { return CheckedItems.ToArray(); }
        }


        private IItemDatabase m_ItemDatabase = null;

        public IItemDatabase ItemDatabase
        {
            get
            {
                return m_ItemDatabase;
            }
            set
            {
                m_SelectedObject = null;
                m_SelectedIndex = 0;
                
                if (m_ItemDatabase != null)
                {
                    m_ItemDatabase.OnInitialReadDone -= ItemDatabaseOnInitialReadDone;
                }

                if (value != null)
                {
                    value.OnInitialReadDone += ItemDatabaseOnInitialReadDone;
                }
                
                m_ItemDatabase = value;

                ZoomStack.Clear();
               
                foreach (LogGraphCurveItem item in CurveList)
                {
                    item.Database = value;
                }
            }
        }

        private void ItemDatabaseOnInitialReadDone(object sender, EventArgs e)
        {
            var itemsIDs = m_ItemDatabase.GetItemsIDs().ToList();

            CurveList
                .Select(x => (string)x.Tag)
                .Where(x => !itemsIDs.Contains(x))
                .ToList()
                .ForEach(RemoveItem);
        }

        public bool OnGraphMouseMove(ZedGraphControl sender, MouseEventArgs e)
        {

            return false;
        }

        public bool OnGraphMouseDown(ZedGraphControl sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                m_RightClickPosition = e.Location;

            object selectedObject;

            using (Graphics g = sender.CreateGraphics())
            {
                FindNearestObject(e.Location, g, out selectedObject, out m_SelectedIndex);
            }

            SetSelectedObject(selectedObject);

            return false;
        }

        public void SetSelectedObject(object selectedObject)
        {
            if (selectedObject == m_SelectedObject)
                return;

            if (m_SelectedObject is LogGraphCurveItem oldCurve)
            {
                oldCurve.Color = m_SelectedObjectOriginalColor;
            }

            m_SelectedObject = selectedObject;

            if (m_SelectedObject is LogGraphCurveItem curve)
            {
                m_SelectedObjectOriginalColor = curve.Color;
                curve.Color = Color.Red;

                var index = CurveList.IndexOf(curve);
                var newIndex = CurveList.Move(index, -index);
            }

            OnCurveItemSelected?.Invoke(this, EventArgs.Empty);

            m_Parent.UpdateGraph();
        }

        public void OnMenuBuilder(ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState)
        {
            if (m_ItemDatabase == null)
                return;
            menuStrip.Items.Add(new ToolStripLabel("Threshold:"));
            ToolStripTextBox setThreshold = new ToolStripTextBox("Threshold");
            setThreshold.Text = m_Threshold?.Location.Y.ToString();

            setThreshold.KeyPress += (o, args) =>
            {
                if (((Keys)args.KeyChar) == Keys.Return)
                {
                    if (double.TryParse(setThreshold.Text, out var value))
                    {
                        SetThreshold(value);
                    }
                    else
                    {
                        SetThreshold(null);
                    }
                }
            };
            menuStrip.Items.Add(setThreshold);

            ToolStripMenuItem sendToModule = new ToolStripMenuItem("Send to module");
            sendToModule.Enabled = !string.IsNullOrEmpty(SelectedItem);


            IModule[] modules = m_Manager.GetModules();

            foreach (IModule module in modules)
            {
                if (module.SupportsSendToCalls)
                {
                    ToolStripMenuItem m = new ToolStripMenuItem(module.DisplayName, null, new EventHandler(OnSendToModule));
                    m.Tag = module;
                    sendToModule.DropDownItems.Add(m);
                }
            }

            menuStrip.Items.Add(sendToModule);

            menuStrip.Items.Add(new ToolStripSeparator());

            if (m_SelectedObject is LogGraphCurveItem selectedLine)
            {
                BuildLineMenu(selectedLine, menuStrip, DashStyle.Solid, DashStyle.Dash, DashStyle.Dot, DashStyle.DashDot, DashStyle.DashDotDot);
            }
            else
            {
                BuildAxisMenu(XAxis, menuStrip, "X-Axis", AxisType.Date, AxisType.DateAsOrdinal);
                BuildAxisMenu(YAxis, menuStrip, "Y-Axis", AxisType.Linear, AxisType.LinearAsOrdinal, AxisType.Log);
                //BuildAxisMenu(X2Axis, menuStrip, "X2-Axis");
                BuildAxisMenu(Y2Axis, menuStrip, "Y2-Axis", AxisType.Linear, AxisType.LinearAsOrdinal, AxisType.Log);
            }

            ToolStripLabel label = new ToolStripLabel("Grid item on X-Axis:");
            label.Font = new Font(label.Font, FontStyle.Underline);
            menuStrip.Items.Add(label);

            ToolStripComboBox maps = new ToolStripComboBox("Maps");
            maps.SelectedIndexChanged += new EventHandler(OnMappedItemChanged);
            maps.AutoSize = true;
            maps.DropDownStyle = ComboBoxStyle.DropDownList;
            maps.DropDownWidth = 300;

            maps.Items.Add("None");
            maps.ToolTipText = $"Only values with less than {MaxEntriesInCurveToPopulateGridItems} entries are available.";
            maps.AutoToolTip = true;

            foreach (var curve in CurveList.Cast<LogGraphCurveItem>())
            {
                if (m_ItemDatabase.GetItemRowCount(curve.ItemID) > MaxEntriesInCurveToPopulateGridItems)
                    continue;

                maps.Items.Add(curve.ItemID);
                if (curve.ItemID == m_MappedItem)
                    maps.SelectedIndex = maps.Items.Count - 1;
            }

            menuStrip.Items.Add(maps);
        }

        private void OnSendToModule(object sender, EventArgs e)
        {
            CurveItem item;
            int index;
            if (this.FindNearestPoint(m_RightClickPosition, out item, out index))
            {
                DateTime timeStamp = DateTime.FromOADate(item.Points[index].X);
                m_Manager.SendTo(((IModule)((ToolStripMenuItem)sender).Tag), new string[] { SelectedItem }, timeStamp);
            }
        }

        private void OnMappedItemChanged(object sender, EventArgs e)
        {
            ToolStripComboBox combo = sender as ToolStripComboBox;

            if (combo.SelectedIndex == 0)
                SetMappedItem(string.Empty);
            else
                SetMappedItem(combo.SelectedItem.ToString());
        }

        private void BuildLineMenu(LogGraphCurveItem lineItem, ContextMenuStrip menuStrip, params DashStyle[] styles)
        {
            ToolStripMenuItem item;

            item = new ToolStripMenuItem("Show Symbol", null, new EventHandler(OnShowSymbol));
            item.Checked = lineItem.Symbol.IsVisible;
            item.CheckOnClick = true;
            item.Tag = lineItem;
            menuStrip.Items.Add(item);

            item = new ToolStripMenuItem("Show last value infinitely", null, new EventHandler(OnShowLastValueInfinitely));
            item.Checked = lineItem.DrawInfinitely;
            item.CheckOnClick = true;
            item.Tag = lineItem;
            menuStrip.Items.Add(item);

            item = new ToolStripMenuItem("Line Style");

            menuStrip.Items.Add(item);

            foreach (DashStyle style in styles)
            {
                ToolStripMenuItem subItem = new ToolStripMenuItem(style.ToString());
                subItem.Tag = lineItem;
                subItem.Click += new EventHandler(OnStyleClicked);
                item.DropDownItems.Add(subItem);
                if (style == lineItem.Line.Style)
                    subItem.Checked = true;
            }

            //item = new ToolStripMenuItem("Asign to X2-Axis", null, new EventHandler(OnAssignToX2Axis));
            //item.Enabled = X2Axis.IsVisible;
            //item.Checked = lineItem.IsX2Axis;
            //item.Tag = lineItem;
            //menuStrip.Items.Add(item);

            string label = lineItem.IsY2Axis ? "Assign to Y-Axis" : "Assign to Y2-Axis";
            item = new ToolStripMenuItem(label, null, new EventHandler(OnAssignToY2Axis))
            {
                Enabled = Y2Axis.IsVisible,
                Checked = lineItem.IsX2Axis,
                Tag = lineItem
            };
            menuStrip.Items.Add(item);

            item = new ToolStripMenuItem("Export to CSV...", null, new EventHandler(OnExportToCSV));
            item.Tag = lineItem;
            menuStrip.Items.Add(item);
        }

        private void OnShowLastValueInfinitely(object sender, EventArgs e)
        {
            m_HasChanged = true;
            ((LogGraphCurveItem)((ToolStripMenuItem)sender).Tag).DrawInfinitely =
                ((ToolStripMenuItem)sender).Checked;

            m_Parent.UpdateGraph();
        }

        private void OnStyleClicked(object sender, EventArgs e)
        {
            m_HasChanged = true;

            ((LineItem)((ToolStripMenuItem)sender).Tag).Line.Style = (DashStyle)Enum.Parse(typeof(DashStyle), ((ToolStripMenuItem)sender).Text);
            m_Parent.UpdateGraph();
        }

        private void OnExportToCSV(object sender, EventArgs args)
        {
            string id = ((LogGraphCurveItem)((ToolStripMenuItem)sender).Tag).ItemID;

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Title = Resources.ExportCSVSpecifyFile;
                dlg.DefaultExt = "*.csv";
                dlg.Filter = Resources.ExportCSVFilter;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        m_ItemDatabase.ExportToCSV(dlg.FileName, id, ',');
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, Resources.ExportCSVFailed);
                    }
                }
            }
        }

        private void OnShowSymbol(object sender, EventArgs args)
        {
            ((LineItem)((ToolStripMenuItem)sender).Tag).Symbol.IsVisible = !((LineItem)((ToolStripMenuItem)sender).Tag).Symbol.IsVisible;
            m_HasChanged = true;
            m_Parent.UpdateGraph();
        }

        private void OnAssignToX2Axis(object sender, EventArgs args)
        {
            ((LineItem)((ToolStripMenuItem)sender).Tag).IsX2Axis = !((LineItem)((ToolStripMenuItem)sender).Tag).IsX2Axis;
            m_HasChanged = true;
            AxisChange();
            m_Parent.UpdateGraph();
        }

        private void OnAssignToY2Axis(object sender, EventArgs args)
        {
            var lineItem = (LineItem)((ToolStripMenuItem)sender).Tag;
            string itemID = lineItem.Tag.ToString();
            lineItem.IsY2Axis = !lineItem.IsY2Axis;
            lineItem.Label.Text = lineItem.IsY2Axis ? itemID + " (Y2)" : itemID;
            m_HasChanged = true;
            AxisChange();
            m_Parent.UpdateGraph();
            CheckItemRenderingCapability(itemID);
        }

        private void BuildAxisMenu(Axis axis, ContextMenuStrip menuStrip, string name, params AxisType[] axisTypes)
        {
            ToolStripMenuItem item;
            ToolStripMenuItem dropItem;

            item = new ToolStripMenuItem(name);
            menuStrip.Items.Add(item);

            ToolStripLabel label = new ToolStripLabel("Title");
            label.Font = new Font(label.Font, FontStyle.Underline);
            item.DropDownItems.Add(label);

            ToolStripTextBox tb = new ToolStripTextBox(axis.Title.Text);

            tb.Text = axis.Title.Text;
            tb.TextChanged += new EventHandler(OnAxisTitleChanged);
            tb.Tag = axis;
            item.DropDownItems.Add(tb);

            dropItem = new ToolStripMenuItem("Visible", null, new EventHandler(OnAxisVisible));
            dropItem.Tag = axis;
            dropItem.Checked = axis.IsVisible;

            item.DropDownItems.Add(dropItem);

            ToolStripMenuItem format = new ToolStripMenuItem("Format");

            item.DropDownItems.Add(format);

            foreach (AxisType axisType in axisTypes)
            {
                dropItem = new ToolStripMenuItem(axisType.ToString(), null, new EventHandler(OnAxisTypeChanged));
                dropItem.Name = axisType.ToString();
                dropItem.Tag = axis;
                dropItem.Checked = axis.Type == axisType;

                format.DropDownItems.Add(dropItem);
            }
        }

        private void OnAxisTitleChanged(object sender, EventArgs e)
        {
            ((Axis)((ToolStripTextBox)sender).Tag).Title.Text = ((ToolStripTextBox)sender).Text;
            m_HasChanged = true;
            m_Parent.UpdateGraph();
        }

        private void OnAxisVisible(object sender, EventArgs args)
        {
            ((Axis)((ToolStripMenuItem)sender).Tag).IsVisible = !((Axis)((ToolStripMenuItem)sender).Tag).IsVisible;
            m_HasChanged = true;
            AxisChange();
            m_Parent.UpdateGraph();
        }

        private void OnAxisTypeChanged(object sender, EventArgs args)
        {
            Axis axis = (Axis)((ToolStripMenuItem)sender).Tag;
            axis.Type = (AxisType)Enum.Parse(typeof(AxisType), ((ToolStripMenuItem)sender).Name);
            m_HasChanged = true;
            if (axis == YAxis || axis == Y2Axis)
            {
                CheckItemRenderingCapability(CheckedItems);
            }
            AxisChange();
            m_Parent.UpdateGraph();
        }

        public string SelectedItem
        {
            get
            {
                if (m_SelectedObject is LineItem)
                    return ((LogGraphCurveItem)m_SelectedObject).ItemID;
                else
                    return string.Empty;
            }
            set
            {
                CurveItem item = GetCurve(value);

                SetSelectedObject(item);

                if (OnCurveItemSelected != null)
                    OnCurveItemSelected(this, EventArgs.Empty);
            }
        }

        private CurveItem GetCurve(string value)
        {
            CurveItem item = CurveList.Find(new Predicate<CurveItem>(
                delegate (CurveItem line)
                {
                    return ((LogGraphCurveItem)line).ItemID == value;
                }));
            return item;
        }

        public void Clear()
        {
            CurveList.Clear();
            m_HasChanged = true;
        }

        internal void LoadSettings(PaneSettings ps)
        {
            ps.GridLineItem = m_MappedItem;
            LoadAxisSettings(ps.XAxis, XAxis);
            LoadAxisSettings(ps.X2Axis, X2Axis);
            LoadAxisSettings(ps.YAxis, YAxis);
            LoadAxisSettings(ps.Y2Axis, Y2Axis);

            foreach (ItemSettings item in ps.Items)
            {
                item.ID = FixItemID(item.ID);
                CheckedItems.RaiseListChangedEvents = false;
                CheckedItems.Add(item.ID);
                CheckedItems.RaiseListChangedEvents = true;
                LogGraphCurveItem logitem = AddCuveItem(item.ID, Color.FromArgb(item.Color));

                logitem.IsX2Axis = item.AssignedToX2Axis;
                logitem.IsY2Axis = item.AssignedToY2Axis;
                logitem.Symbol.IsVisible = item.ShowSymbol;
                logitem.DrawInfinitely = item.DrawInfinitely;
            }

            SetThreshold((string.IsNullOrWhiteSpace(ps.Threshold) ? null : (double?)double.Parse(ps.Threshold)));

            m_HasChanged = false;
            AxisChange();
        }

        //The wrong ItemID was saved in the settings and this method is used to correct those errors.
        private string FixItemID(string p)
        {
            int index = p.IndexOf(' ');
            if (index > 0)
                return p.Substring(0, index);

            return p;
        }

        internal void LoadAxisSettings(AxisSettings settings, Axis axis)
        {
            axis.Title.Text = settings.Title;
            axis.IsVisible = settings.IsVisible;
            axis.Type = (AxisType)Enum.Parse(typeof(AxisType), settings.Format);
        }

        internal void SaveSettings(PaneSettings ps)
        {
            SetMappedItem(ps.GridLineItem);
            SaveAxisSettings(ps.XAxis, XAxis);
            SaveAxisSettings(ps.X2Axis, X2Axis);
            SaveAxisSettings(ps.YAxis, YAxis);
            SaveAxisSettings(ps.Y2Axis, Y2Axis);

            foreach (LogGraphCurveItem item in CurveList)
            {
                ItemSettings settings = new ItemSettings();

                settings.ID = item.ItemID;
                settings.AssignedToX2Axis = item.IsX2Axis;
                settings.AssignedToY2Axis = item.IsY2Axis;
                settings.ShowSymbol = item.Symbol.IsVisible;
                settings.Color = item.Color.ToArgb();
                settings.DrawInfinitely = item.DrawInfinitely;
                ps.Items.Add(settings);
            }

            ps.Threshold = m_Threshold?.Location.Y.ToString();

            m_HasChanged = false;
        }

        private void SetMappedItem(string itemID)
        {
            if (m_MappedItem == itemID)
                return;

            GraphObjList.Clear();

            CurveItem item = GetCurve(m_MappedItem);

            if (item != null)
                item.IsVisible = true;

            m_MappedItem = itemID;

            item = CurveList[itemID];

            if (item != null)
            {
                item.IsVisible = false;
                List<GraphObj> objects = new List<GraphObj>();

                for (int index = 0; index < item.Points.Count; index++)
                {
                    PointPair point = item[index];

                    LineObj line = new LineObj(Color.Gray, point.X, 0, point.X, 1);
                    line.Tag = point.Y;
                    line.Location.CoordinateFrame = CoordType.XScaleYChartFraction;
                    line.ZOrder = ZOrder.E_BehindCurves;


                    TextObj text = new TextObj(point.Y.ToString(), point.X, 1);
                    text.Location.CoordinateFrame = CoordType.XScaleYChartFraction;
                    text.IsClippedToChartRect = false;
                    text.FontSpec.Border.IsVisible = false;
                    text.Location.AlignV = AlignV.Bottom;
                    text.Location.AlignH = AlignH.Right;
                    text.FontSpec.Angle = -90;
                    text.FontSpec.Fill.IsVisible = false;
                    objects.Add(line);
                    objects.Add(text);
                }

                GraphObjList.AddRange(objects);
            }

            m_HasChanged = true;
            m_Parent.UpdateGraph();
        }

        internal void SaveAxisSettings(AxisSettings settings, Axis axis)
        {
            settings.Title = axis.Title.Text;
            settings.IsVisible = axis.IsVisible;
            settings.Format = axis.Type.ToString();
        }

        private bool m_HasChanged = false;

        public bool GetHasChanged()
        {
            return m_HasChanged;
        }

        private bool IsBooleanItem(string itemName)
        {
            var item = m_ItemDatabase?.GetFirstItem(itemName, x => x.Value != null);
            return item != null && bool.TryParse(item.Value, out bool value);
        }

        private void CheckItemRenderingCapability(string itemName)
        {
            CheckItemRenderingCapability(new List<string>() { itemName });
        }

        private void CheckItemRenderingCapability(IEnumerable<string> graphItems)
        {
            //boolean attributes can't be rendered in Log scale graphs. (0 is not a valid value)
            var booleanItemsOnLogScale = CurveList.Where(x => x.GetYAxis(this).Scale.IsLog)
                                                  .Select(x => x.Tag.ToString())
                                                  .Intersect(graphItems.Where(x => IsBooleanItem(x)));

            string msg = null;

            if (booleanItemsOnLogScale.Count() == 1)
            {
                msg = string.Format("\"{0}\" is a boolean value which can't be rendered in a Log scale graph. Please change the format of the Y-axis for the item to be rendered correctly.", booleanItemsOnLogScale.First());
            }
            else if (booleanItemsOnLogScale.Count() > 1)
            {
                string itemNames = string.Join("\", \"", booleanItemsOnLogScale.ToArray(), 0, booleanItemsOnLogScale.Count() - 1) + "\" and \"" + booleanItemsOnLogScale.Last();
                msg = string.Format("\"{0}\" are boolean values which can't be rendered in a Log scale graph. Please change the format of the Y-axis for the items to be rendered correctly.", itemNames);
            }

            if (msg != null)
            {
                MessageBox.Show(msg, "Rendering incompability warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
