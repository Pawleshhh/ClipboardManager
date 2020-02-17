using System;
using System.Collections.Generic;

namespace ManiacClipboard.Model
{
    /// <summary>
    /// Represents multiple data that can be stored on the clipboard.
    /// </summary>
    public abstract class CollectionClipboardData<T> : ClipboardData<IReadOnlyCollection<T>>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardData"/> class.
        /// </summary>
        /// <param name="data">Collection of data to be stored.</param>
        /// <param name="type">Type of the data.</param>
        /// <exception cref="ArgumentNullException">Throws when data is null.</exception>
        /// <exception cref="ArgumentException">Throws when type is not defined.</exception>
        protected CollectionClipboardData(IReadOnlyCollection<T> data, ClipboardDataType type) : base(data, type)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardData"/> class.
        /// </summary>
        /// <param name="data">Collection of data to be stored.</param>
        /// <param name="type">Type of the data.</param>
        /// <param name="copyTime">Date and time when data was stored.</param>
        /// <exception cref="ArgumentNullException">Throws when data is null.</exception>
        /// <exception cref="ArgumentException">Throws when type is not defined.</exception>
        protected CollectionClipboardData(IReadOnlyCollection<T> data, ClipboardDataType type, DateTime copyTime) : base(data, type, copyTime)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardData"/> class.
        /// </summary>
        /// <param name="data">Collection of data to be stored.</param>
        /// <param name="type">Type of the data.</param>
        /// <param name="source">Source where the stored data comes from.</param>
        /// <exception cref="ArgumentNullException">Throws when data is null.</exception>
        /// <exception cref="ArgumentException">Throws when type is not defined.</exception>
        protected CollectionClipboardData(IReadOnlyCollection<T> data, ClipboardDataType type, ClipboardSource source) : base(data, type, source)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardData"/> class.
        /// </summary>
        /// <param name="data">Collection of data to be stored.</param>
        /// <param name="type">Type of the data.</param>
        /// <param name="copyTime">Date and time when data was stored.</param>
        /// <param name="source">Source where the stored data comes from.</param>
        /// <exception cref="ArgumentNullException">Throws when data is null.</exception>
        /// <exception cref="ArgumentException">Throws when type is not defined.</exception>
        protected CollectionClipboardData(IReadOnlyCollection<T> data, ClipboardDataType type, DateTime copyTime, ClipboardSource source) : base(data, type, copyTime, source)
        {
        }

        #endregion Constructors

        #region Public methods

        /// <summary>
        /// Splits stored collection to specified array of clipboard data.
        /// </summary>
        public abstract ClipboardData[] Split();

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj is CollectionClipboardData<T> collection)
                return CollectionHelper.ScrambledEquals(Data, collection.Data);

            return false;
        }

        public override int GetHashCode()
        {
            //Code from: https://stackoverflow.com/questions/10567450/implement-gethashcode-for-objects-that-contain-collections
            int hc = 0;
            foreach (var item in Data)
                hc ^= item.GetHashCode();
            return hc;
        }

        public override string ToString()
        {
            return $"Collection of {typeof(T).Name}";
        }

        #endregion Public methods
    }
}