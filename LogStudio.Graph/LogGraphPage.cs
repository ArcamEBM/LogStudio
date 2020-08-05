using LogStudio.Data;
using LogStudio.Framework;
using LogStudio.Graph;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ZedGraph;

namespace LogStudio
{
    public partial class LogGraphPage : TabPage
    {
        public event EventHandler OnSelectedPaneChanged;
        public event EventHandler OnSelectedCurveItemChanged;

        private ZedGraphControl m_Graph;

        private LogGraphPane m_SelectedPane = null;

        public readonly bool ReadOnly;
        private IModuleManager m_Manager;
        public LogGraphPage(IModuleManager manager, bool readOnly, bool addPane)
        {
            InitializeComponent();
            m_Manager = manager;
            ReadOnly = readOnly;
            m_Graph = new ZedGraphControlEx();
            m_Graph.Dock = DockStyle.Fill;
            m_Graph.Margin = new Padding(0);
            m_Graph.Padding = new Padding(0);
            m_Graph.GraphPane.XAxis.Type = AxisType.Date;
            m_Graph.IsShowPointValues = true;

            m_Graph.ContextMenuBuilder += new ZedGraphControl.ContextMenuBuilderEventHandler(OnMenuBuilder);

            m_Graph.MouseDownEvent += new ZedGraphControl.ZedMouseEventHandler(OnGraphMouseDown);
            m_Graph.MouseMoveEvent += new ZedGraphControl.ZedMouseEventHandler(OnGraphMouseMove);
            m_Graph.MasterPane.Border.IsVisible = false;
            m_Graph.MasterPane.PaneList.Clear();
            m_Graph.PointValueEvent += new ZedGraphControl.PointValueHandler(OnGraphPointValue);
            Controls.Add(m_Graph);

            if (addPane)
                AddPane(false);
        }

        public string OnGraphPointValue(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            PointPair point = curve.Points[iPt];
            return $"{curve.Label.Text}: ({DateTime.FromOADate(point.X).ToString("yyyy-MM-dd HH:mm:ss.fff")}, {point.Y})";
        }

        public bool OnGraphMouseMove(ZedGraphControl sender, MouseEventArgs e)
        {
            if (m_SelectedPane != null)
                m_SelectedPane.OnGraphMouseMove(sender, e);

            return false;
        }

        private bool OnGraphMouseDown(ZedGraphControl sender, MouseEventArgs e)
        {

            LogGraphPane pane = sender.MasterPane.FindPane(e.Location) as LogGraphPane;
            SelectedPane = pane;

            if (pane == null)
                return false;

            return pane.OnGraphMouseDown(sender, e);
        }

        public void UpdateCompleteGraph()
        {
            using (Graphics g = m_Graph.CreateGraphics())
            {
                m_Graph.MasterPane.SetLayout(g, PaneLayout.SquareRowPreferred);
                m_Graph.MasterPane.DoLayout(g);
            }
            m_Graph.AxisChange();
            m_Graph.Invalidate();
        }


        private IItemDatabase m_Database = null;

        public IItemDatabase ItemDatabase
        {
            get
            {
                return m_Database;
            }
            set
            {
                m_Database = value;

                foreach (LogGraphPane pane in m_Graph.MasterPane.PaneList)
                {
                    pane.ItemDatabase = m_Database;
                }

                m_Graph.MasterPane.Title.IsVisible = m_Database != null;
                if (m_Database != null)
                    m_Graph.MasterPane.Title.Text = Path.GetFileNameWithoutExtension(m_Database.Filename);
                else
                    m_Graph.MasterPane.Title.Text = string.Empty;

                UpdateCompleteGraph();
            }
        }

        public void ClearAllGraphs()
        {
            List<LogGraphPane> panes = new List<LogGraphPane>();

            foreach (LogGraphPane pane in m_Graph.MasterPane.PaneList)
            {
                panes.Add(pane);
            }

            foreach (LogGraphPane pane in panes)
            {
                RemovePane(pane);
            }

            UpdateCompleteGraph();

            m_HasChanged = true;
        }

        private void OnMenuBuilder(ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState)
        {
            ToolStripMenuItem item = new ToolStripMenuItem("Remove Selected Graph", LogStudio.Graph.Properties.Resources.RemoveGraph, new EventHandler(OnRemoveSelectedPaneClicked));
            item.Enabled = m_SelectedPane != null;
            item.ImageScaling = ToolStripItemImageScaling.None;
            item.ImageTransparentColor = Color.Fuchsia;

            menuStrip.Items.Insert(0, new ToolStripSeparator());
            menuStrip.Items.Insert(0, item);

            item = new ToolStripMenuItem("Add Graph", LogStudio.Graph.Properties.Resources.AddGraph, new EventHandler(OnAddPaneClicked));
            item.ImageScaling = ToolStripItemImageScaling.None;
            item.ImageTransparentColor = Color.Fuchsia;
            menuStrip.Items.Insert(0, item);

            menuStrip.Items.Add(new ToolStripSeparator());

            item = new ToolStripMenuItem("Syncronize X-Axis", null, new EventHandler(OnSyncXAxis));
            item.Checked = m_Graph.IsSynchronizeXAxes;
            menuStrip.Items.Add(item);

            item = new ToolStripMenuItem("Syncronize Y-Axis", null, new EventHandler(OnSyncYAxis));
            item.Checked = m_Graph.IsSynchronizeYAxes;
            menuStrip.Items.Add(item);

            if (m_SelectedPane != null)
                m_SelectedPane.OnMenuBuilder(sender, menuStrip, mousePt, objState);
        }

        private void OnSyncXAxis(object sender, EventArgs args)
        {
            m_HasChanged = true;
            m_Graph.IsSynchronizeXAxes = !m_Graph.IsSynchronizeXAxes;
            if (m_Graph.IsSynchronizeXAxes)
            {
                UpdateCompleteGraph();
            }
        }

        private void OnSyncYAxis(object sender, EventArgs args)
        {
            m_HasChanged = true;
            m_Graph.IsSynchronizeYAxes = !m_Graph.IsSynchronizeYAxes;
            UpdateCompleteGraph();
        }

        private void OnAddPaneClicked(object sender, EventArgs args)
        {
            AddPane(true);
        }

        private void OnRemoveSelectedPaneClicked(object sender, EventArgs args)
        {
            RemoveSelectedPane();
        }

        public LogGraphPane SelectedPane
        {
            get
            {
                return m_SelectedPane;
            }
            set
            {
                if (m_SelectedPane == value)
                    return;

                if (m_SelectedPane != null)
                {
                    m_SelectedPane.Border.Color = Color.Black;
                    m_SelectedPane.Border.Width = 1;
                    m_SelectedPane.SelectedItem = string.Empty;
                }

                m_SelectedPane = value;

                if (m_SelectedPane != null)
                {
                    m_SelectedPane.Border.Color = Color.Red;
                    m_SelectedPane.Border.Width = 2;
                }

                if (OnSelectedPaneChanged != null)
                    OnSelectedPaneChanged(this, EventArgs.Empty);

                m_Graph.Invalidate();
            }
        }

        public LogGraphPane AddPane(bool markAsChanged)
        {
            LogGraphPane pane = new LogGraphPane(this, m_Manager);
            pane.Title.IsVisible = false;
            pane.ItemDatabase = m_Database;
            pane.OnCurveItemSelected += new EventHandler(OnCurveSelected);

            m_Graph.MasterPane.Add(pane);

            UpdateCompleteGraph();

            if (markAsChanged)
                m_HasChanged = true;
            return pane;
        }

        private void OnCurveSelected(object sender, EventArgs e)
        {
            if (OnSelectedCurveItemChanged != null)
                OnSelectedCurveItemChanged(sender, e);
        }

        public void RemoveSelectedPane()
        {
            RemovePane(m_SelectedPane);
        }

        public void RemovePane(LogGraphPane pane)
        {
            if (pane != null)
            {
                pane.OnCurveItemSelected -= new EventHandler(OnCurveSelected);
                m_Graph.MasterPane.PaneList.Remove(pane);

                if (m_SelectedPane == pane)
                    m_SelectedPane = null;

                if (OnSelectedPaneChanged != null)
                    OnSelectedPaneChanged(this, EventArgs.Empty);

                UpdateCompleteGraph();

                m_HasChanged = true;
            }
        }

        public bool NameAlreadyExists(string name)
        {
            return m_Graph.MasterPane.PaneList.Any(pane => pane.Title.Text == name);
        }


        internal void Print(Graphics g, RectangleF rect)
        {
            m_Graph.MasterPane.ReSize(g, rect);
            m_Graph.MasterPane.DoLayout(g);
            m_Graph.MasterPane.Draw(g);

            using (Graphics graphics = CreateGraphics())
            {
                m_Graph.MasterPane.ReSize(graphics, ClientRectangle);
                m_Graph.MasterPane.DoLayout(graphics);
            }

            m_Graph.Invalidate();
        }

        internal void LoadSettings(PageSettings page)
        {
            m_Graph.IsSynchronizeXAxes = page.IsSynchronizeXAxes;
            m_Graph.IsSynchronizeYAxes = page.IsSynchronizeYAxes;

            foreach (PaneSettings ps in page.Panes)
            {
                LogGraphPane pane = AddPane(false);
                pane.Title.Text = ps.Name;

                pane.LoadSettings(ps);
            }
            m_HasChanged = false;
        }

        internal void SaveSettings(PageSettings pageSettings)
        {
            pageSettings.IsSynchronizeXAxes = m_Graph.IsSynchronizeXAxes;
            pageSettings.IsSynchronizeYAxes = m_Graph.IsSynchronizeYAxes;

            foreach (LogGraphPane pane in m_Graph.MasterPane.PaneList)
            {
                PaneSettings ps = new PaneSettings();
                ps.Name = pane.Title.Text;

                pane.SaveSettings(ps);

                pageSettings.Panes.Add(ps);
            }
            m_HasChanged = false;
        }

        internal void UpdateGraph()
        {
            m_Graph.Invalidate();
        }

        internal Graphics GetGraphics()
        {
            return m_Graph.CreateGraphics();
        }

        internal void UpdateGraph(RectangleF rect)
        {
            m_Graph.Invalidate(Rectangle.Round(rect));
        }

        private bool m_HasChanged = false;

        public bool GetHasChanged()
        {
            bool changed = m_HasChanged;

            if (changed)
                return changed;

            if (m_Graph.MasterPane?.PaneList != null)
            {
                foreach (var pane in m_Graph.MasterPane.PaneList.Cast<LogGraphPane>())
                {
                    if (pane.GetHasChanged())
                    {
                        changed = true;
                        break;
                    }
                }
            }

            return changed;
        }

        internal void ResetAutoScale(LogGraphPane logGraphPane)
        {
            m_Graph.RestoreScale(logGraphPane);
        }

        internal void Selected()
        {
            if (m_SelectedPane == null && m_Graph.MasterPane.PaneList.Count > 0)
            {
                m_SelectedPane = (LogGraphPane)m_Graph.MasterPane[0];

                m_SelectedPane.Border.Color = Color.Red;
                m_SelectedPane.Border.Width = 2;
            }
        }

        internal void SelectFirstPane()
        {
            if (m_Graph.MasterPane.PaneList.Count > 0)
                SelectedPane = (LogGraphPane)m_Graph.MasterPane.PaneList[0];
        }
    }
}
