using System;
using System.Collections.Generic;
using System.Linq;

namespace LogStudio.Data
{
    public class ItemProperties : IEnumerable<KeyValuePair<string, string>>
    {
        private Dictionary<string, string> m_Properties = new Dictionary<string, string>();

        internal ItemProperties(string row)
        {
            string[] valuePairs = row.TrimEnd('\r', '\n').Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string valuePair in valuePairs)
            {
                string[] pair = valuePair.Split('=');

                if (pair.Length == 2)
                {
                    m_Properties.Add(pair[0], pair[1]);
                }
            }
        }

        public string this[string name]
        {
            get
            {
                return m_Properties[name];
            }
        }

        public string[] GetPropertyNames()
        {
            return m_Properties.Keys.ToArray<string>();
        }

        public int Count
        {
            get
            {
                return m_Properties.Count;
            }
        }

        public bool TryGetValue(string name, out string value)
        {
            return m_Properties.TryGetValue(name, out value);
        }

        #region IEnumerable<KeyValuePair<string,string>> Members

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return m_Properties.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_Properties.GetEnumerator();
        }

        #endregion
    }
}
