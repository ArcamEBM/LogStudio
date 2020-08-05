using System;

namespace LogStudio.Data
{
    public enum ProgressStateEnum
    {
        Starting,
        InProgress,
        Monitoring,
        Decompressing,
        Done
    }

    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(ProgressStateEnum state)
            : this(state, 0, TimeSpan.Zero, 0)
        {
        }

        public ProgressEventArgs(ProgressStateEnum state, TimeSpan elapsedTime)
            : this(state, 0, elapsedTime, 0)
        {
        }

        public ProgressEventArgs(ProgressStateEnum state, int progress, TimeSpan elapsedTime, float bytesPerSecond)
        {
            State = state;
            Progress = progress;
            ElapsedTime = elapsedTime;
            BytesPerSecond = bytesPerSecond;
        }

        public ProgressStateEnum State { get; private set; }

        public int Progress { get; private set; }

        public TimeSpan ElapsedTime { get; private set; }

        public float BytesPerSecond { get; private set; }
    }
}
