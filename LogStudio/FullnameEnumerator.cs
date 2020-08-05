using LogStudio.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LogStudio
{
    [DebuggerDisplay("{m_OpenArrayFullname}")]
    public class FullnameEnumerator : IEnumerable<string>
    {
        private string m_OpenArrayFullname;
        public FullnameEnumerator(string openArrayFullname, IItemDatabase database)
        {
            m_OpenArrayFullname = openArrayFullname;
            Database = database;
        }

        public IItemDatabase Database { get; set; }

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            IList<string> fullnames = ExpandOpenArray(m_OpenArrayFullname);
            return fullnames.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private IList<string> ExpandOpenArray(string arrayItem)
        {
            List<string> fullnames = new List<string>();


            if (!arrayItem.Contains("[]"))
            {
                fullnames.Add(arrayItem);
                return fullnames;
            }
            else
            {
                string[] elements = arrayItem.Split(new string[] { "[]" }, StringSplitOptions.None);
                int[] indexes = new int[elements.Length - 1];
                CreateOpenArrayItems(fullnames, elements, ref indexes, 0);
            }

            return fullnames;
        }

        private void CreateOpenArrayItems(IList<string> fullnames, string[] elements, ref int[] indexes, int index)
        {
            while (true)
            {
                if (index < indexes.Length - 1)
                {
                    CreateOpenArrayItems(fullnames, elements, ref indexes, index + 1);
                    indexes[index]++;

                    string path = CreatePath(elements, indexes);

                    if (!Database.Exists(path))
                        return;
                }
                else
                {
                    string path = CreatePath(elements, indexes);

                    if (Database.Exists(path))
                    {
                        fullnames.Add(path);
                        indexes[index]++;
                    }
                    else
                    {
                        for (int i = index; i < indexes.Length; i++)
                        {
                            indexes[i] = 0;
                        }

                        return;
                    }
                }
            }
        }

        private string CreatePath(string[] elements, int[] indexes)
        {
            string path = "";

            for (int index = 0; index < elements.Length; index++)
            {
                if (index > 0)
                    path += string.Format("[{0}]", indexes[index - 1]);
                path += elements[index];
            }

            return path;
        }
    }
}
