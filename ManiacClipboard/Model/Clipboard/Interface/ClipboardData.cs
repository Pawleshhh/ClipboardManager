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
        /// <param name="type">Type of the data.</param>
        /// <exception cref="ArgumentNullException">Throws when data is null.</exception>
        /// <exception cref="ArgumentException">Throws when type is not defined.</exception>
        protected ClipboardData(object data, ClipboardDataType type)
        {
            if (data == null)
                throw new ArgumentNullException("data", "The data parameter cannot be null.");
            if (!Enum.IsDefined(typeof(ClipboardDataType), type))
                throw new ArgumentException("The type parameter's value must be defined.", "type");

            Data = data;
            DataType = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardData"/> class.
        /// </summary>
        /// <param name="data">Data to be stored.</param>
        /// <param name="type">Type of the data.</param>
        /// <param name="copyTime">Date and time when data was stored.</param>
        /// <exception cref="ArgumentNullException">Throws when data is null.</exception>
        /// <exception cref="ArgumentException">Throws when type is not defined.</exception>
        protected ClipboardData(object data, ClipboardDataType type, DateTime copyTime) : this(data, type)
        {
            CopyTime = copyTime;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets stored data.
        /// </summary>
        public object Data { get; }

        /// <summary>
        /// Gets type of stored data.
        /// </summary>
        public ClipboardDataType DataType { get; }

        /// <summary>
        /// Gets date and time when data was stored.
        /// </summary>
        public DateTime CopyTime { get; } = DateTime.Now;

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
