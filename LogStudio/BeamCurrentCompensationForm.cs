using LogStudio.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ZedGraph;

namespace LogStudio
{
    public partial class BeamCurrentCompensationForm : Form
    {
        private readonly IItemDatabase m_Database;
        public BeamCurrentCompensationForm(IItemDatabase database)
        {
            InitializeComponent();
            m_Database = database;
            InitializeBeamCurrentCompensation();
        }

        private void InitializeBeamCurrentCompensation()
        {
            string currents = "Measurements.BeamCurrentFeedback.Settings.Elements.Pair[].Current";
            string feedback = "Measurements.BeamCurrentFeedback.Settings.Elements.Pair[].Feedback";
            string[] allCurrents = new FullnameEnumerator(currents, m_Database).ToArray();
            string[] allFeedback = new FullnameEnumerator(feedback, m_Database).ToArray();
            if (!m_Database.Exists("Measurements.BeamCurrentFeedback.Settings.Enabled"))
                throw new KeyNotFoundException("Cannot find beam current feedback compensation data");

            if (allCurrents.Length != allFeedback.Length)
                throw new ArgumentException("Error with beam current feedback compensation array");

            LogRowDataPoint[][] dataCurrent =
                allCurrents.Select(currId => m_Database.GetAllDP(currId).ToArray()).ToArray();
            LogRowDataPoint[][] dataFeedback =
                allFeedback.Select(fbId => m_Database.GetAllDP(fbId).ToArray()).ToArray();

            const string applyFeedbackCompensationId = "Process.CalibrationControl.ApplyBeamFeedbackCompensation";

            LogRowData[] allApplyTimes = m_Database.GetAll(applyFeedbackCompensationId).ToArray();
            List<LogRowData> applyTimes = new List<LogRowData>();
            if (!Boolean.Parse(allApplyTimes[0].Value))
                applyTimes.Add(allApplyTimes[0]);
            applyTimes.AddRange(allApplyTimes.Where(p => Boolean.Parse(p.Value)));

            TimeSelectionListBox.Items.AddRange(applyTimes.ToArray());

            TimeSelectionListBox.SelectedValueChanged += (oo, ee) =>
            {
                var box = (ListBox)oo;
                var data = (LogRowData)box.SelectedItem;
                DateTime time = data.TimeStamp;

                GraphsSplitContainer.Panel1.Controls.Clear();

                double[] currentSnapshot = dataCurrent.GetDataPoints(time).Select(p => p.Value).ToArray();
                var feedbackSnapshot = dataFeedback.GetDataPoints(time).Select(p => p.Value).ToArray();

                ZedGraphControl zed = CalculatedGraphs.PlotBeamCurrentFeedback(
                    CalculatedGraphs.GetBeamCurrentFeedbackDeviation(currentSnapshot, feedbackSnapshot));
                zed.Dock = DockStyle.Fill;
                zed.IsShowPointValues = true;
                zed.GraphPane.YAxis.Scale.Max = 3;
                zed.GraphPane.YAxis.Scale.Min = -3;
                zed.AxisChange();

                ZedGraphControl zed2 =
                    CalculatedGraphs.PlotBeamCurrentFeedback(
                        CalculatedGraphs.GetBeamCurrentFeedback(currentSnapshot, feedbackSnapshot));
                zed2.Dock = DockStyle.Fill;
                zed2.GraphPane.YAxis.Title.Text = "feedback (mA)";
                zed2.IsShowPointValues = true;
                zed2.GraphPane.YAxis.Scale.Max = 50;
                zed2.GraphPane.YAxis.Scale.Min = 0;
                zed2.AxisChange();

                var panel = new SplitContainer { Dock = DockStyle.Fill };
                GraphsSplitContainer.Panel1.Controls.Add(panel);
                panel.Panel1.Controls.Add(zed2);
                panel.Panel2.Controls.Add(zed);
            };

            TimeSelectionListBox.SelectedIndex = 0;
        }
    }
    internal static class Extensions
    {
        internal static LogRowDataPoint[] GetDataPoints(this LogRowDataPoint[][] data, DateTime time)
        {
            return data.Select(array => array.Last(t => t.TimeStamp <= time)).ToArray();
        }
    }
}
