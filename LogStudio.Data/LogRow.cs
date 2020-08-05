using System;

namespace LogStudio.Data
{
    [Serializable]
    public abstract class LogRow
    {
        public DateTime TimeStamp { get; set; }



        protected LogRow()
        {
        }

        protected LogRow(DateTime timeStamp)
        {
            TimeStamp = timeStamp;
        }
    }
}
