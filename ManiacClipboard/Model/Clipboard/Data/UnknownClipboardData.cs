using System;

namespace ManiacClipboard.Model
{
    /// <summary>
    /// Represents data that can be stored on the clipboard and its type is unknown.
    /// </summary>
    public sealed class UnknownClipboardData : ClipboardData<object>
    {
        #region Constructors

        public UnknownClipboardData(object data, string[] formats) : base(data, ClipboardDataType.Unknown)
        {
            Initialize(formats);
        }

        public UnknownClipboardData(object data, string[] formats, DateTime copyTime) : base(data, ClipboardDataType.Unknown, copyTime)
        {
            Initialize(formats);
        }

        public UnknownClipboardData(object data, string[] formats, ClipboardSource source) : base(data, ClipboardDataType.Unknown, source)
        {
            Initialize(formats);
        }

        public UnknownClipboardData(object data, string[] formats, DateTime copyTime, ClipboardSource source) : base(data, ClipboardDataType.Unknown, copyTime, source)
        {
            Initialize(formats);
        }

        private void Initialize(string[] formats)
        {
            _formats = formats ?? new string[0];
            _isDisposable = Data is IDisposable;

            if (!_isDisposable)
                GC.SuppressFinalize(this);
        }

        #endregion Constructors

        #region Private fields

        private string[] _formats;

        private string _name;

        private bool _isDisposed;

        private bool _isDisposable;

        #endregion Private fields

        #region Methods

        /// <summary>
        /// Gets array of formats of the unknown stored data.
        /// </summary>
        public string[] GetFormats()
        {
            string[] array = new string[_formats.Length];

            Array.Copy(_formats, 0, array, 0, _formats.Length);

            return array;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj is UnknownClipboardData data)
                return Data.Equals(data.Data);

            return false;
        }

        public override int GetHashCode() => Data.GetHashCode() * 13 * 17;

        public override string ToString()
        {
            if (_name == null)
                _name = Data.GetType().Name;

            return _name;
        }

        public override void Dispose()
        {
            if (_isDisposable && !_isDisposed)
            {
                ((IDisposable)Data).Dispose();
                _isDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        ~UnknownClipboardData()
        {
            Dispose();
        }

        #endregion Methods
    }
}