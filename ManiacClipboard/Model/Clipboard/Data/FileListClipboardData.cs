using System;
using System.Collections.Generic;
using System.Linq;

namespace ManiacClipboard.Model
{
    /// <summary>
    /// Contains data with multiple paths to files and directories that can be stored on the clipboard.
    /// </summary>
    public class FileListClipboardData : CollectionClipboardData<KeyValuePair<string, bool>>
    {
        #region Constructors

        protected FileListClipboardData(IReadOnlyCollection<KeyValuePair<string, bool>> data, ClipboardDataType type) :
            base(data, type)
        {
        }

        protected FileListClipboardData(IReadOnlyCollection<KeyValuePair<string, bool>> data, ClipboardDataType type, DateTime copyTime) :
            base(data, type, copyTime)
        {
        }

        protected FileListClipboardData(IReadOnlyCollection<KeyValuePair<string, bool>> data, ClipboardDataType type, ClipboardSource source) :
            base(data, type, source)
        {
        }

        protected FileListClipboardData(IReadOnlyCollection<KeyValuePair<string, bool>> data, ClipboardDataType type, DateTime copyTime, ClipboardSource source) :
            base(data, type, copyTime, source)
        {
        }

        public FileListClipboardData(IReadOnlyCollection<KeyValuePair<string, bool>> data) :
            this(data, ClipboardDataType.FileList)
        {
        }

        public FileListClipboardData(IReadOnlyCollection<KeyValuePair<string, bool>> data, DateTime copyTime) :
            this(data, ClipboardDataType.FileList, copyTime)
        {
        }

        public FileListClipboardData(IReadOnlyCollection<KeyValuePair<string, bool>> data, ClipboardSource source) :
            this(data, ClipboardDataType.FileList, source)
        {
        }

        public FileListClipboardData(IReadOnlyCollection<KeyValuePair<string, bool>> data, DateTime copyTime, ClipboardSource source) :
            this(data, ClipboardDataType.FileList, copyTime, source)
        {
        }

        #endregion Constructors

        #region Methods

        public override ClipboardData[] Split()
            => Data.Select(n => new PathClipboardData(n.Key, n.Value, CopyTime, Source) { KeepThat = this.KeepThat }).ToArray();

        public override string ToString()
        {
            return "Collection of paths";
        }

        #endregion Methods
    }
}