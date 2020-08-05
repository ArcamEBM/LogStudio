using LogStudio.Data;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace LogStudio
{
    public class LogGraphCurveItem : LineItem, IDisposable
    {
        private LogGraphPane m_Pane;

        private ISynchronizeInvoke m_SynchronizeInvoke;

        private int m_Ordered = 0;

        public LogGraphCurveItem(string itemID, LogGraphPane pane, ISynchronizeInvoke synchronizeInvoke)
            : base(itemID)
        {
            m_ItemID = itemID;
            m_SynchronizeInvoke = synchronizeInvoke;

            m_Pane = pane;
            Symbol.Type = SymbolType.Star;
            Symbol.IsVisible = false;
            this.Line.DashOn = 100;
            this.Line.DashOff = 10;
            this.Line.IsOptimizedDraw = true;
            this.Line.IsSmooth = false;
            this.Line.IsAntiAlias = false;
            this.IsSelectable = true;
            this.Line.StepType = StepType.ForwardStep;
            IsBoolean = true;
            DrawInfinitely = false;
        }

        private string m_ItemID;
        public string ItemID
        {
            get { return m_ItemID; }
        }

        public bool IsBoolean { get; set; }

        public bool DrawInfinitely { get; set; }

        private IItemDatabase m_Database;
        public IItemDatabase Database
        {
            get
            {
                return m_Database;
            }
            set
            {
                if (m_Database != null)
                {
                    m_Database.OnIndexesChanged -= new EventHandler<LogIndexChangesEventArgs>(OnIndexChanges);
                    m_Database.OnIndexesAdded -= new EventHandler<LogIndexAddedEventArgs>(OnIndexAdded);
                }

                Clear();

                m_Database = value;
                m_Ordered = 0;

                if (m_Database != null)
                {
                    m_Database.OnIndexesChanged += new EventHandler<LogIndexChangesEventArgs>(OnIndexChanges);
                    m_Database.OnIndexesAdded += new EventHandler<LogIndexAddedEventArgs>(OnIndexAdded);
                    FetchItemData(ItemID);
                }
            }
        }

        private void OnIndexAdded(object sender, LogIndexAddedEventArgs e)
        {
            ItemProperties properties = null;

            if (Array.Exists<string>(e.AddedIndexes, x => string.CompareOrdinal(x, ItemID) == 0))
                properties = m_Database.GetItemProperties(ItemID);

            if (properties != null)
            {
                string unit = properties["Unit"];

                if (!string.IsNullOrEmpty(unit))
                    Label.Text = string.Format("{0} ({1})", ItemID, unit);
            }
        }

        public override void Draw(Graphics g, GraphPane pane, int pos, float scaleFactor)
        {
            Line.DrawCurve(g, pane, this, scaleFactor);
            DrawLineToRightEdge(g, pane);
            if (Symbol.IsVisible)
                Symbol.Draw(g, pane, this, scaleFactor, IsSelected);
        }

        private void DrawLineToRightEdge(Graphics graphics, GraphPane pane)
        {
            if (!DrawInfinitely || this.Points.Count == 0)
                return;

            using (var pen = Line.GetPen(pane, 1))
            {
                var rightMost = pane.Chart.Rect.Right;

                var lastPoint = this.Points[this.Points.Count - 1];

                var lastPointX = pane.XAxis.Scale.Transform(lastPoint.X);

                if (lastPointX >= rightMost)
                    return;

                var yScale = (_isY2Axis) ? pane.Y2Axis.Scale : pane.YAxis.Scale;
                var lastPointY = yScale.Transform(lastPoint.Y);

                graphics.DrawLine(pen, lastPointX, lastPointY, rightMost, lastPointY);
            }
        }

        public new void Clear()
        {
            base.Clear();
        }

        private void FetchItemData(string itemID)
        {
            if (!m_Database.Exists(itemID))
                return;

            int count = m_Database.GetItemRowCount(itemID) - m_Ordered;

            if (count > 0)
            {
                int startIndex = m_Ordered;

                m_Ordered += count;

                m_Database.BeginGetItemRowsDP(itemID, 10000, startIndex, count, new QueuedReadRequestEventHandler(OnDataArrival));
            }
        }

        private void OnDataArrival(long instanceID, string itemID, LogRowDataPoint[] points)
        {
            if (m_SynchronizeInvoke.InvokeRequired)
            {
                m_SynchronizeInvoke.BeginInvoke(new QueuedReadRequestEventHandler(OnDataArrival), new object[] { instanceID, itemID, points });
                return;
            }

            if (m_Database == null || m_Database.InstanceID != instanceID)
                return;

            foreach (LogRowDataPoint point in points)
            {
                AddPoint(point.TimeStamp.ToOADate(), point.Value);
            }

            m_Pane.AxisChange();

            var startPoint = (float)Math.Max(m_Pane.XAxis.Scale.ReverseTransform((float)points[0].TimeStamp.ToOADate()) - 1, m_Pane.Rect.Left);
            var endPoint = (float)Math.Min(m_Pane.XAxis.Scale.ReverseTransform((float)points[points.Length - 1].TimeStamp.ToOADate()) + 2, m_Pane.Rect.Right);

            var top = m_Pane.Rect.Top;
            var bottom = m_Pane.Rect.Bottom;

            RefreshParent(new RectangleF(startPoint, top, endPoint - startPoint, bottom));
        }
        public override void GetRange(out double xMin, out double xMax, out double yMin, out double yMax, bool ignoreInitial, bool isBoundedRanges, GraphPane pane)
        {
            // initialize the values to outrageous ones to start
            yMin = Double.MaxValue;
            yMax = Double.MinValue;
            xMin = Double.MaxValue;
            xMax = Double.MinValue;

            Axis yAxis = this.GetYAxis(pane);
            if (yAxis == null)
                return;


            bool isYOrdinal = yAxis.Scale.IsAnyOrdinal;
            bool isYLog = yAxis.Scale.IsLog;

            // Loop over each point in the arrays
            //foreach ( PointPair point in this.Points )
            PointPairList list = (PointPairList)this.Points;
            int count = list.Count;

            if (count < 2)
                return;

            xMin = list[0].X;
            xMax = list[count - 1].X;

            for (int i = 0; i < count; i++)
            {
                var point = list[i];

                double curY = isYOrdinal ? i : point.Y;

                //0 is not a valid value for logarithmic scales. Only add values if greater than zero or if the scale is not logarithmic.
                if (!isYLog || curY > 0)
                {
                    if (curY < yMin)
                        yMin = curY;
                    if (curY > yMax)
                        yMax = curY;
                }
            }
        }

        private void RefreshParent(RectangleF clip)
        {
            if (Points.Count == 0)
                return;

            bool needAxisUpdate = !m_Pane.Chart.Rect.Contains(clip);

            if (needAxisUpdate || !m_Pane.IsZoomed)
            {
                m_Pane.AxisChange();
                m_Pane.Parent.UpdateGraph(m_Pane.Parent.ClientRectangle);
            }
            else
            {
                m_Pane.Parent.UpdateGraph(clip);
            }
        }

        void OnIndexChanges(object sender, LogIndexChangesEventArgs e)
        {
            if (m_Pane.Parent.InvokeRequired)
            {
                m_Pane.Parent.BeginInvoke(new EventHandler<LogIndexChangesEventArgs>(OnIndexChanges), sender, e);
                return;
            }

            foreach (LogIndexChangeRange change in e.Changes)
            {
                if (change.ItemID == ItemID)
                {
                    FetchItemData(ItemID);
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Database = null;
            Clear();
        }

        #endregion
    }
}
