using NUnit.Framework;
using System.Windows.Forms;

namespace LogStudio.Test
{
    [TestFixture]
    public class CalculatedGraphsTest
    {
        [Test]
        // Need more improvements
        public void BeamCurrentFeedbackTest()
        {
            double[] currentSnapshot = new double[] { 0, 5, 10, 15, 0, 0 };
            double[] feedbackSnapshot = new double[] { 0, 5, 11, 15, 0, 0 };
            var zed2 = CalculatedGraphs.PlotBeamCurrentFeedback(CalculatedGraphs.GetBeamCurrentFeedbackDeviation(currentSnapshot, feedbackSnapshot));
            zed2.Dock = DockStyle.Fill;
            zed2.GraphPane.YAxis.Title.Text = "feedback (mA)";

            Form f = new Form() { Width = 800, Height = 550 };
            SplitContainer panel = new SplitContainer { Dock = DockStyle.Fill };
            f.Controls.Add(panel);
            panel.Panel1.Controls.Add(zed2);
        }
    }
}
