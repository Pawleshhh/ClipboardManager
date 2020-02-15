using System;

namespace ManiacClipboard.Model
{
    /// <summary>
    /// Represents data that can be stored on the clipboard.
    /// </summary>
    public abstract class ClipboardData : IDisposable
    {
        #region Constructors

        protected ClipboardData(object data)
        {

        }

        #endregion

        #region Properties

        #endregion

        #region Public methods

        /// <summary>
        /// Disposes stored data.
        /// </summary>
        public virtual void Dispose() { }

        #endregion

    }
}
