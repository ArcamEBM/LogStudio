using LogStudio.Reader.Parser;
using NUnit.Framework;

namespace LogStudio.Reader.Test
{
    [TestFixture]
    public class ExpressionParserTests
    {
        [Test]
        [Ignore("Need fix")]
        public void SplitIntoParts()
        {
            var onChange =
                "[OnCondition(Process.RakeControl.MoveToOtherSideDone && MoveToOtherSideSet && InternalProcessManagerState != ProcessState.Stopped && (ProcessManagerState == ProcessState.Running OR ProcessManagerState == ProcessState.Paused) && !IsRestarting && !Analyse.Control.LowGridBeamCalibration)] Process.ProcessManager.OnMelt2() (Logic)";
            var expression = ExpressionParser.ParseExpression(onChange);
        }
    }
}
