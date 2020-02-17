using ManiacClipboard.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ManiacClipboard.ViewModel
{
    public class ImageClipboardDataViewModel : ClipboardDataViewModel<MemoryStream>
    {

        #region Constructors

        public ImageClipboardDataViewModel(ImageClipboardData clipboardData) : base(clipboardData)
        {

        }

        #endregion

        #region Public methods

        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();
            GC.SuppressFinalize(this);
        }

        ~ImageClipboardDataViewModel()
        {
            Dispose();
        }

        #endregion

    }
}
