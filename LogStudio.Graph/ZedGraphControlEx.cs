using System.Windows.Forms;
using ZedGraph;

namespace LogStudio.Graph
{
    public class ZedGraphControlEx : ZedGraphControl
    {
        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                SetStyle(ControlStyles.UserPaint, true);
                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                return base.CreateParams;
            }
        }
    }
}
