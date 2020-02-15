using System;

namespace ManiacClipboard.Model
{
    /// <summary>
    /// Represents data that can be stored on the clipboard.
    /// </summary>
    public abstract class ClipboardData : IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardData"/> class.
        /// </summary>
        /// <param name="data">Data to be stored.</param>
        protected ClipboardData(object data, ClipboardDataType type)
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

    /// <summary>
    /// Enum values that indicate certain clipboard data type.
    /// </summary>
    public enum ClipboardDataType
    {
        /// <summary>
        /// Unknown data.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Text data.
        /// </summary>
        Text,
        /// <summary>
        /// Text which contains path to specified file.
        /// </summary>
        FilePath,
        /// <summary>
        /// Collection of paths to specified files.
        /// </summary>
        FileList,
        /// <summary>
        /// Image data.
        /// </summary>
        Image,
        /// <summary>
        /// Audio data.
        /// </summary>
        Audio
    }
}
