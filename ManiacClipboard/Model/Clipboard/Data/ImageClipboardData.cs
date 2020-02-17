using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace ManiacClipboard.Model
{
    /// <summary>
    /// Stores image data that can be stored on the clipboard.
    /// </summary>
    public class ImageClipboardData : ClipboardData<MemoryStream>, IEquatable<ImageClipboardData>
    {

        #region Constructors

        protected ImageClipboardData(MemoryStream data, ClipboardDataType type) : base(data, type)
        {
        }

        protected ImageClipboardData(MemoryStream data, ClipboardDataType type, DateTime copyTime) : base(data, type, copyTime)
        {
        }

        protected ImageClipboardData(MemoryStream data, ClipboardDataType type, ClipboardSource source) : base(data, type, source)
        {
        }

        protected ImageClipboardData(MemoryStream data, ClipboardDataType type, DateTime copyTime, ClipboardSource source) : base(data, type, copyTime, source)
        {
        }

        public ImageClipboardData(MemoryStream data) : base(data, ClipboardDataType.Image)
        {
        }

        public ImageClipboardData(MemoryStream data, DateTime copyTime) : base(data, ClipboardDataType.Image, copyTime)
        {
        }

        public ImageClipboardData(MemoryStream data, ClipboardSource source) : base(data, ClipboardDataType.Image, source)
        {
        }

        public ImageClipboardData(MemoryStream data, DateTime copyTime, ClipboardSource source) : base(data, ClipboardDataType.Image, copyTime, source)
        {
        }

        #endregion

        #region Private fields

        private bool _isDisposed;

        private int _hashCode;

        private bool _hashCodeCalculated;

        #endregion

        #region Public methods

        public bool Equals([AllowNull] ImageClipboardData other)
        {
            return other != null && GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj is ImageClipboardData img)
                return Equals(img);

            return false;
        }

        public override int GetHashCode()
        {
            if(!_hashCodeCalculated)
            {
                string bStr = Convert.ToBase64String(Data.ToArray());
                _hashCode = bStr.GetHashCode() * 27;
                _hashCodeCalculated = true;
            }

            return _hashCode;
        }

        public override void Dispose()
        {
            if (!_isDisposed)
            {
                Data.Dispose();
                GC.SuppressFinalize(this);
                _isDisposed = true;
            }
        }

        ~ImageClipboardData()
        {
            Dispose();
        }

        #endregion

    }
}
