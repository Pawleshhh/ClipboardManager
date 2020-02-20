using System;

namespace ManiacClipboard.Model
{
    /// <summary>
    /// Represents data that can be stored on the clipboard.
    /// </summary>
    public abstract class ClipboardData : IClipboardData
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardData"/> class.
        /// </summary>
        /// <param name="data">Data to be stored.</param>
        /// <param name="type">Type of the data.</param>
        /// <param name="source">Source where the stored data comes from.</param>
        /// <exception cref="ArgumentNullException">Throws when data is null.</exception>
        /// <exception cref="ArgumentException">Throws when type is not defined.</exception>
        protected ClipboardData(object data, ClipboardDataType type, ClipboardSource source) : this(data, type)
        {
            Source = source;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardData"/> class.
        /// </summary>
        /// <param name="data">Data to be stored.</param>
        /// <param name="type">Type of the data.</param>
        /// <param name="copyTime">Date and time when data was stored.</param>
        /// <param name="source">Source where the stored data comes from.</param>
        /// <exception cref="ArgumentNullException">Throws when data is null.</exception>
        /// <exception cref="ArgumentException">Throws when type is not defined.</exception>
        protected ClipboardData(object data, ClipboardDataType type, DateTime copyTime, ClipboardSource source) : this(data, type, copyTime)
        {
            Source = source;
        }

        #endregion Constructors

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

        /// <summary>
        /// Gets the source where the stored data comes from. Can be null!
        /// </summary>
        public ClipboardSource Source { get; }

        /// <summary>
        /// Gets or sets the tag name of stored data.
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// Gets or sets whether the stored data is supposed to be kept or not.
        /// </summary>
        public bool KeepThat { get; set; }

        #endregion Properties

        #region Public methods

        public abstract override bool Equals(object obj);

        public abstract override int GetHashCode();

        public override string ToString() => Data.ToString();

        /// <summary>
        /// Disposes stored data.
        /// </summary>
        public virtual void Dispose() { }

        #endregion Public methods
    }

    /// <summary>
    /// Represents data that can be stored on the clipboard.
    /// </summary>
    /// <typeparam name="T">Type of stored data.</typeparam>
    public abstract class ClipboardData<T> : ClipboardData, IClipboardData<T>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardData"/> class.
        /// </summary>
        /// <param name="data">Data to be stored.</param>
        /// <param name="type">Type of the data.</param>
        /// <exception cref="ArgumentNullException">Throws when data is null.</exception>
        /// <exception cref="ArgumentException">Throws when type is not defined.</exception>
        protected ClipboardData(T data, ClipboardDataType type) : base(data, type)
        {
            Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardData"/> class.
        /// </summary>
        /// <param name="data">Data to be stored.</param>
        /// <param name="type">Type of the data.</param>
        /// <param name="copyTime">Date and time when data was stored.</param>
        /// <exception cref="ArgumentNullException">Throws when data is null.</exception>
        /// <exception cref="ArgumentException">Throws when type is not defined.</exception>
        protected ClipboardData(T data, ClipboardDataType type, DateTime copyTime) : base(data, type, copyTime)
        {
            Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardData"/> class.
        /// </summary>
        /// <param name="data">Data to be stored.</param>
        /// <param name="type">Type of the data.</param>
        /// <param name="source">Source where the stored data comes from.</param>
        /// <exception cref="ArgumentNullException">Throws when data is null.</exception>
        /// <exception cref="ArgumentException">Throws when type is not defined.</exception>
        protected ClipboardData(T data, ClipboardDataType type, ClipboardSource source) : base(data, type, source)
        {
            Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardData"/> class.
        /// </summary>
        /// <param name="data">Data to be stored.</param>
        /// <param name="type">Type of the data.</param>
        /// <param name="copyTime">Date and time when data was stored.</param>
        /// <param name="source">Source where the stored data comes from.</param>
        /// <exception cref="ArgumentNullException">Throws when data is null.</exception>
        /// <exception cref="ArgumentException">Throws when type is not defined.</exception>
        protected ClipboardData(T data, ClipboardDataType type, DateTime copyTime, ClipboardSource source) : base(data, type, copyTime, source)
        {
            Data = data;
        }

        #endregion Constructors

        #region Properties

        public new T Data { get; }

        #endregion Properties
    }
}