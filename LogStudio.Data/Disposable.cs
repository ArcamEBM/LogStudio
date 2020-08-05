using System;

namespace LogStudio.Data
{
    /// <summary>
    /// Summary description for Disposable.
    /// </summary>
    /// 
    [Serializable]
    public class Disposable : IDisposable
    {
        /// <summary>
        /// Contructor
        /// </summary>
        public Disposable()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// Implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                OnDispose();
            }
            OnUnManagedDispose();
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// </summary>
        ~Disposable()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }

        /// <summary>
        /// Override this method to be able to dispose managed data
        /// </summary>
        protected virtual void OnDispose()
        {
        }

        /// <summary>
        /// Override this method to be able to dispose unmanaged data
        /// </summary>
        protected virtual void OnUnManagedDispose()
        {
        }
    }
}
