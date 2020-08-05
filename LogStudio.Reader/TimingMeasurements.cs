using LogStudio.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ZedGraph;

namespace LogStudio.Reader
{
    public partial class TimingMeasurements : UserControl
    {
        private ZedGraphControl m_Ctrl;
        private IItemDatabase m_Database;

        public event EventHandler<PointClickedEventArgs> OnPointClicked;

        public TimingMeasurements()
        {
            InitializeComponent();

            m_Ctrl = new ZedGraphControl { Dock = DockStyle.Fill };
            m_Ctrl.ContextMenuBuilder += new ZedGraphControl.ContextMenuBuilderEventHandler(PaneContextMenuBuilder);
            var pane = m_Ctrl.GraphPane;

            m_Ctrl.MouseDownEvent += (sender, args) =>
            {
                if (pane.FindNearestPoint(new PointF(args.X, args.Y), out var curve, out var index))
                {
                    var point = curve.Points[index];
                    var closestTime = DateTime.FromOADate(point.X);
                    OnPointClicked?.Invoke(this, new PointClickedEventArgs(m_StartNameValue.Key, closestTime));
                    return true;
                }

                return false;
            };

            m_Ctrl.ZoomEvent += (sender, state, newState) =>
            {
                UpdateCurveInfo(pane);
            };

            m_Ctrl.PointDateFormat = "yyyy-MM-dd HH:mm:ss.fff";
            m_Ctrl.PointValueFormat = "N2";

            pane.XAxis.Type = AxisType.Date;
            pane.XAxis.Title.Text = "Timestamp";
            pane.YAxis.Title.Text = "Time (s)";
            pane.YAxis.Scale.FormatAuto = true;
            pane.Legend.IsVisible = false;
            m_Ctrl.IsShowPointValues = true;

            Controls.Add(m_Ctrl);
        }

        private void PaneContextMenuBuilder(ZedGraphControl control, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState)
        {
            foreach (ToolStripMenuItem item in menuStrip.Items)
            {
                // Remove "Undo all zoom/pan" since it's not working
                if ((string)item.Tag == "undo_all")
                {
                    menuStrip.Items.Remove(item);
                    break;
                }
            }
        }

        public IItemDatabase Database
        {
            get => m_Database;
            set
            {
                if (m_Database != null)
                    m_Database.OnInitialReadDone -= DatabaseOnOnInitialReadDone;

                m_Database = value;

                if (m_Database != null)
                    m_Database.OnInitialReadDone += DatabaseOnOnInitialReadDone;
            }
        }

        private void DatabaseOnOnInitialReadDone(object sender, EventArgs e)
        {
            if (InvokeRequired)
                Invoke(new Action(Calculate));
        }

        private KeyValuePair<string, string> m_StartNameValue;

        public void SetStart(string name, string value)
        {
            m_StartNameValue = new KeyValuePair<string, string>(name, value);
        }

        private KeyValuePair<string, string> m_EndNameValue;

        public void SetEnd(string name, string value)
        {
            m_EndNameValue = new KeyValuePair<string, string>(name, value);
        }

        public void Calculate()
        {
            PointPairList points = new PointPairList();

            var pane = m_Ctrl.GraphPane;

            pane.CurveList.Clear();
            pane.GraphObjList.Clear();

            if (m_Database != null && !string.IsNullOrEmpty(m_StartNameValue.Value) && !string.IsNullOrEmpty(m_EndNameValue.Key))
            {
                var startItems = m_Database.FindAll(m_StartNameValue.Key, data => data.Value == m_StartNameValue.Value);

                var endItems = m_Database.FindAll(m_EndNameValue.Key, data => data.Value == m_EndNameValue.Value);

                var ordered = startItems.Concat(endItems).OrderBy(data => data.TimeStamp).ToList();

                LogRowData start = null;

                foreach (var logRowData in ordered)
                {
                    if (start == null && logRowData.ItemID == m_StartNameValue.Key &&
                        logRowData.Value == m_StartNameValue.Value)
                        start = logRowData;

                    if (start != null && logRowData.ItemID == m_EndNameValue.Key &&
                        logRowData.Value == m_EndNameValue.Value && start != logRowData)
                    {
                        var seconds = (logRowData.TimeStamp - start.TimeStamp).TotalSeconds;
                        points.Add(new PointPair(start.OaTimeStamp, seconds));

                        start = null;
                    }
                }
            }

            pane.AddCurve("", points, Color.DodgerBlue);

            m_Ctrl.RestoreScale(pane);
            pane.AxisChange();
            pane.Title.Text = $"{m_StartNameValue.Key}={m_StartNameValue.Value} => {m_EndNameValue.Key}={m_EndNameValue.Value}";

            UpdateCurveInfo(pane);

            m_Ctrl.Refresh();
        }

        private static void UpdateCurveInfo(GraphPane pane)
        {
            pane.GraphObjList.Clear();

            if (((pane.CurveList.Count > 0) ? pane.CurveList[0].Points : null) is PointPairList points && points.Count > 0)
            {
                var minValue = pane.XAxis.Scale.Min;
                var maxValue = pane.XAxis.Scale.Max;

                var filteredPoints = points.Where(p => p.X >= minValue && p.X <= maxValue).ToArray();

                var max = filteredPoints.Max(pair => pair.Y);
                var min = filteredPoints.Min(pair => pair.Y);
                var avg = filteredPoints.Average(pair => pair.Y);


                TextObj text = new TextObj($"Min: {min:N2} s, Max: {max:N2} s, Avg: {avg:N2} s", 0, 0,
                    CoordType.PaneFraction, AlignH.Left, AlignV.Top)
                { FontSpec = { Size = 14 } };

                pane.GraphObjList.Add(text);

                LineObj maxLine = new LineObj(Color.Red, 0, max, 1, max)
                {
                    Location = { CoordinateFrame = CoordType.XChartFractionYScale },
                    IsClippedToChartRect = true
                };

                pane.GraphObjList.Add(maxLine);

                LineObj minLine = new LineObj(Color.Red, 0, min, 1, min)
                {
                    Location = { CoordinateFrame = CoordType.XChartFractionYScale },
                    IsClippedToChartRect = true
                };

                pane.GraphObjList.Add(minLine);
            }
        }
    }

    public class PointClickedEventArgs : EventArgs
    {
        public PointClickedEventArgs(string startItemId, DateTime timestamp)
        {
            StartItemId = startItemId;
            Timestamp = timestamp;
        }

        public string StartItemId { get; }
        public DateTime Timestamp { get; }
    }
}
