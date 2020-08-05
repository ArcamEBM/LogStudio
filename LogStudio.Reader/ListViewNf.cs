using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LogStudio.Reader
{
    public partial class ListViewNf : ListView
    {
        private readonly Stack<ListViewItem> m_History = new Stack<ListViewItem>();

        public ListViewNf()
        {
            InitializeComponent();

            PreviousSelectedItem = null;
        }

        public void ClearHistory()
        {
            m_History.Clear();
        }

        private ListViewItem PreviousSelectedItem { get; set; }

        protected override CreateParams CreateParams
        {
            get
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                return base.CreateParams;
            }
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (!m_IsBackingHistory && PreviousSelectedItem != null)
                m_History.Push(PreviousSelectedItem);

            base.OnSelectedIndexChanged(e);
            PreviousSelectedItem = FocusedItem;
        }

        private bool m_IsBackingHistory;

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyData == Keys.Back)
            {
                if (m_History.Count == 0)
                    return;

                m_IsBackingHistory = true;
                BeginUpdate();
                try
                {
                    ListViewItem item = m_History.Pop();

                    SelectedIndices.Clear();
                    FocusedItem = item;
                    item.Selected = true;
                    item.EnsureVisible();
                }
                finally
                {
                    EndUpdate();
                    m_IsBackingHistory = false;
                }
            }

            base.OnKeyDown(e);
        }
    }
}