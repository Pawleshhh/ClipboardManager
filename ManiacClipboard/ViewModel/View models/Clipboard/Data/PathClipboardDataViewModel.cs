using ManiacClipboard.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManiacClipboard.ViewModel
{
    public class PathClipboardDataViewModel : TextClipboardDataViewModel
    {

        #region Constructors

        public PathClipboardDataViewModel(PathClipboardData clipboardData) : base(clipboardData)
        {
            _pathClipboardData = clipboardData;
        }

        #endregion

        #region Private fields

        private readonly PathClipboardData _pathClipboardData;

        #endregion

        #region Properties

        public bool IsDirectory => _pathClipboardData.IsDirectory;

        #endregion

    }
}
