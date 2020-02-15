using System;
using System.Collections.Generic;
using System.Text;

namespace ManiacClipboard.Model
{

    /// <summary>
    /// Represents data with file path that can be stored on the clipboard.
    /// </summary>
    public class PathClipboardData : TextClipboardData
    {
        #region Constructors

        protected PathClipboardData(string data, ClipboardDataType type, bool directory) : base(data, type)
        {
            IsDirectory = directory;
        }

        protected PathClipboardData(string data, ClipboardDataType type, bool directory, DateTime copyTime) : base(data, type, copyTime)
        {
            IsDirectory = directory;
        }

        protected PathClipboardData(string data, ClipboardDataType type, bool directory, ClipboardSource source) : base(data, type, source)
        {
            IsDirectory = directory;
        }

        protected PathClipboardData(string data, ClipboardDataType type, bool directory, DateTime copyTime, ClipboardSource source) :
            base(data, type, copyTime, source)
        {
            IsDirectory = directory;
        }

        public PathClipboardData(string data, bool directory) : this(data, ClipboardDataType.FilePath, directory)
        {
        }

        public PathClipboardData(string data, bool directory, DateTime copyTime) : this(data, ClipboardDataType.FilePath, directory, copyTime)
        {
        }

        public PathClipboardData(string data, bool directory, ClipboardSource source) :
            this(data, ClipboardDataType.FilePath, directory, source)
        {
        }

        public PathClipboardData(string data, bool directory, DateTime copyTime, ClipboardSource source) :
            this(data, ClipboardDataType.FilePath, directory, copyTime, source)
        {
        }

        #endregion

        #region Properties

        public bool IsDirectory { get; }

        #endregion

    }
}
