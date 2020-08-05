using System;
using System.Windows.Forms;

namespace LogStudio
{
    public partial class PropertyEditorControl : UserControl
    {
        public event EventHandler OnSelectedObjectsChanged;
        public event PropertyValueChangedEventHandler OnPropertyValueChanged;
        private PropertyGrid m_PropertyGrid;

        public PropertyEditorControl()
        {
            InitializeComponent();

            m_PropertyGrid = new PropertyGrid();
            m_PropertyGrid.Dock = DockStyle.Fill;
            m_PropertyGrid.SelectedObjectsChanged += new EventHandler(SelectedObjectsChanged);
            m_PropertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(PropertyValueChanged);
            Controls.Add(m_PropertyGrid);
        }

        private void PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (OnPropertyValueChanged != null)
                OnPropertyValueChanged(s, e);
        }

        private void SelectedObjectsChanged(object sender, EventArgs e)
        {
            if (OnSelectedObjectsChanged != null)
                OnSelectedObjectsChanged(this, EventArgs.Empty);
        }

        public object SelectedObject
        {
            get
            {
                return m_PropertyGrid.SelectedObject;
            }
            set
            {
                m_PropertyGrid.SelectedObject = value;
            }
        }

        public object[] SelectedObjects
        {
            get
            {
                return m_PropertyGrid.SelectedObjects;
            }
            set
            {
                m_PropertyGrid.SelectedObjects = value;
            }
        }
    }
}
