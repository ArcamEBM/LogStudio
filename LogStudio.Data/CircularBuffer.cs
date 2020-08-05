using System;
using System.IO;
using System.IO.Pipes;

namespace LogStudio.Data
{
    public class CircularBuffer : IDisposable
    {
        private AnonymousPipeServerStream m_Server;
        private AnonymousPipeClientStream m_Client;
        private bool m_Started = false;

        public CircularBuffer()
        {
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (!m_Started)
                return;

            m_Server.Write(buffer, offset, count);
        }

        public void Start(int bufferSize)
        {
            m_Server = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.None, bufferSize * 4);
            m_Client = new AnonymousPipeClientStream(PipeDirection.In, m_Server.GetClientHandleAsString());
            m_Started = true;
        }

        public void Stop()
        {
            m_Server.WaitForPipeDrain();
            m_Server.Close();
            m_Client.Close();
            m_Client.Dispose();
            m_Client = null;
            m_Server.Dispose();
            m_Server = null;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            if (!m_Started)
                return 0;

            int read = 0;

            try
            {
                read = m_Client.Read(buffer, offset, count);
            }
            catch (NullReferenceException)
            {
                //Somtimes a nullreference occures when closing down application
            }

            return read;
        }

        public void WaitForEmptyBuffer()
        {
            m_Server.WaitForPipeDrain();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (m_Client != null)
            {
                m_Client.Close();
                m_Client.Dispose();
            }
            if (m_Server != null)
            {
                m_Server.DisposeLocalCopyOfClientHandle();
                m_Server.Close();
                m_Server.Dispose();
            }
        }

        #endregion
    }
}
