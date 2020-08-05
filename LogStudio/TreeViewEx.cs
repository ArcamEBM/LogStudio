using System.Windows.Forms;

namespace LogStudio
{
    public partial class TreeViewEx : TreeView
    {
        private const uint WM_PAINT = 0x000F;
        private const uint WM_LBUTTONDBLCLK = 0x203;

        public TreeViewEx()
        {
            InitializeComponent();
            RaiseAfterCheckEvent = true;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_PAINT && PreventRepaint)
                return;

            base.WndProc(ref m);
        }

        public bool RaiseAfterCheckEvent { get; set; }
        public bool PreventRepaint { get; set; }

        protected override void OnAfterCheck(TreeViewEventArgs e)
        {
            if (RaiseAfterCheckEvent)
                base.OnAfterCheck(e);
        }

        public void ExpandeNodePath(TreeNode node)
        {
            TreeNode n = node;

            while (n != null)
            {
                n.Expand();
                n = n.Parent;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                return base.CreateParams;
            }
        }
    }
}
