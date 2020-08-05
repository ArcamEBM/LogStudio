using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace LogStudio.Reader
{
    public partial class TabbedLogReaderControl : TabControl, INotifyPropertyChanged
    {
        private Data.IItemDatabase m_Database;

        public Data.IItemDatabase Database
        {
            set
            {
                m_Database = value;
                foreach (TabPage tabPage in TabPages)
                {
                    var c = (LogReaderControl)tabPage.Controls[0];
                    c.Database = m_Database;
                }
            }
        }

        public TabbedLogReaderControl()
        {
            InitializeComponent();

            ControlAdded += (sender, args) =>
            {
                var index = Controls.IndexOf(args.Control);
                args.Control.Text = index.ToString("00");
            };

            ControlRemoved += (sender, args) =>
            {
                foreach (Control control in Controls)
                {
                    var index = Controls.IndexOf(control);
                    control.Text = index.ToString("00");
                }
            };
        }

        public TabPage AddPage()
        {
            var tp = new TabPage();
            var reader = new LogReaderControl { Dock = DockStyle.Fill };
            tp.Controls.Add(reader);
            TabPages.Add(tp);
            Selected += OnTabSelected;
            reader.PropertyChanged += reader_PropertyChanged;
            reader.Initialize();
            reader.Database = m_Database;
            //SelectedTab = tp;
            return tp;
        }

        void reader_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        public void Initialize()
        {
        }

        public void OnTabSelected(object sender, TabControlEventArgs e)
        {
            var reader = SelectedTab.Controls[0] as LogReaderControl;
            if (m_Reader != reader)
            {
                m_Reader = reader;
                NotifyOnChangedItems();
            }
        }

        public void OnAdd(object sender, EventArgs args)
        {
            AddPage();
        }

        public void OnRemove(object sender, EventArgs args)
        {
            int index = SelectedIndex;

            if (TabCount == 1)
            {
                AddPage();
            }
            TabPages.RemoveAt(index);

            SelectedIndex = Math.Max(0, index - 1);
        }

        public void GotoTime(string itemId, DateTime timestamp)
        {
            m_Reader?.GotoTime(itemId, timestamp);
        }

        private LogReaderControl m_Reader;
        internal LogReaderControl LogReader { get { return m_Reader; } }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void NotifyOnChangedItems()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CheckedItems"));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedItem"));
        }
    }
}
