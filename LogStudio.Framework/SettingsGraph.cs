using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace LogStudio.Framework
{
    public class SettingsGraph
    {
        public SettingsGraph()
        {
            Pages = new List<PageSettings>();
        }

        public List<PageSettings> Pages { get; set; }
        public TabReaderSettings TabReader { get; set; }


        public static SettingsGraph FromFile(string filename)
        {
            using (Stream stream = File.OpenRead(filename))
            {
                XmlSerializer xs = new XmlSerializer(typeof(SettingsGraph));

                return (SettingsGraph)xs.Deserialize(stream);
            }
        }

        public void ToFile(string filename)
        {
            using (Stream stream = File.Create(filename))
            {
                XmlSerializer xs = new XmlSerializer(typeof(SettingsGraph));

                xs.Serialize(stream, this);
            }
        }
    }

    public class PageSettings
    {
        public PageSettings()
        {
            Panes = new List<PaneSettings>();
        }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public bool IsSynchronizeXAxes { get; set; }

        [XmlAttribute]
        public bool IsSynchronizeYAxes { get; set; }

        public List<PaneSettings> Panes { get; set; }

    }

    public class PaneSettings
    {
        public PaneSettings()
        {
            Items = new List<ItemSettings>();
            Threshold = String.Empty;
            XAxis = new AxisSettings("Time Stamp", "Date", true);
            YAxis = new AxisSettings("Value", "Linear", true);
            X2Axis = new AxisSettings("X2", "Linear", false);
            Y2Axis = new AxisSettings("Y2", "Linear", false);
        }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string GridLineItem { get; set; }

        [XmlAttribute]
        public string Threshold { get; set; }

        public AxisSettings XAxis { get; set; }
        public AxisSettings YAxis { get; set; }
        public AxisSettings X2Axis { get; set; }
        public AxisSettings Y2Axis { get; set; }

        public List<ItemSettings> Items { get; set; }
    }

    public class AxisSettings
    {
        public AxisSettings()
        {
        }

        public AxisSettings(string title, string format, bool visible)
        {
            Title = title;
            Format = format;
            IsVisible = visible;
        }

        [XmlAttribute]
        public string Title { get; set; }

        [XmlAttribute]
        public string Format { get; set; }

        [XmlAttribute]
        public bool IsVisible { get; set; }

    }

    public class TabReaderSettings
    {
        public List<Tab> Tabs { get; set; }
    }

    public class Tab
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement]
        public List<string> IDs { get; set; }
    }


    public class ItemSettings
    {
        public ItemSettings()
        {
            Color = System.Drawing.Color.Blue.ToArgb();
        }

        [XmlAttribute]
        public string ID { get; set; }

        [XmlAttribute]
        public bool AssignedToX2Axis { get; set; }

        [XmlAttribute]
        public bool AssignedToY2Axis { get; set; }

        [XmlAttribute]
        public bool ShowSymbol { get; set; }

        [XmlAttribute]
        public int Color { get; set; }

        [XmlAttribute]
        public bool DrawInfinitely { get; set; }
    }

}
