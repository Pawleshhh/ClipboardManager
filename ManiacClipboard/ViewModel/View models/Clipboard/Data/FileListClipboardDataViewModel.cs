using ManiacClipboard.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManiacClipboard.ViewModel
{
    public class FileListClipboardDataViewModel : CollectionClipboardDataViewModel<KeyValuePair<string, bool>>
    {

        #region Constructors

        protected FileListClipboardDataViewModel(FileListClipboardData clipboardData) : base(clipboardData)
        {
        }

        #endregion

        #region Public methods

        public override ClipboardDataViewModel[] Split()
            => Data.Select(n => new PathClipboardDataViewModel(
                                    new PathClipboardData(n.Key, n.Value, CopyTime, Source))).ToArray();

        #endregion

    }
}
