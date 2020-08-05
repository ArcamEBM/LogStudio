using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ZedGraph;

namespace LogStudio
{
    internal class CalculatedGraphs
    {
        #region beam current feedback compensation

        internal static PointPairList GetBeamCurrentFeedback(double[] demand, double[] feedback)
        {
            if (demand.Length != feedback.Length)
                throw new ArgumentException("nbr demand values != nbr feedback values");

            PointPairList list = new PointPairList();
            for (int i = 0; i < demand.Length; i++)
                list.Add(demand[i], feedback[i]);
            var distinctList = new PointPairList();
            distinctList.AddRange(list.Distinct<PointPair>(new XComparer()).OrderBy(p => p.X));

            return distinctList;
        }

        internal static PointPairList GetBeamCurrentFeedbackDeviation(double[] demand, double[] feedback)
        {
            if (demand.Length != feedback.Length)
                throw new ArgumentException("nbr demand values != nbr feedback values");

            PointPairList list = new PointPairList();
            var distinctList = GetBeamCurrentFeedback(demand, feedback);
            foreach (PointPair t in distinctList.OrderBy(p => p.X))
                list.Add(t.X, t.X - t.Y);

            return list;
        }

        internal class XComparer : IEqualityComparer<PointPair>
        {
            public bool Equals(PointPair x, PointPair y)
            {
                return x.X == y.X;
            }

            public int GetHashCode(PointPair obj)
            {
                return 0;
            }
        }

        internal static ZedGraphControl PlotBeamCurrentFeedback(PointPairList points)
        {
            ZedGraphControl zed = new ZedGraphControl();

            zed.GraphPane.AddCurve(String.Empty, points, Color.DodgerBlue, SymbolType.Circle);
            zed.GraphPane.XAxis.Type = AxisType.Linear;
            zed.GraphPane.Title.Text = "Beam Current Feedback Compensation";
            zed.GraphPane.XAxis.Title.Text = "current mA";
            zed.GraphPane.YAxis.Title.Text = "deviation mA";

            zed.GraphPane.AxisChange();
            return zed;
        }

        #endregion
    }
}
