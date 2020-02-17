
using ManiacClipboard.Model;
using System;

namespace ManiacClipboard.ViewModel
{
    public sealed class UnknownClipboardDataViewModel : ClipboardDataViewModel<object>
    {

        #region Constructors

        public UnknownClipboardDataViewModel(UnknownClipboardData clipboardData) : base(clipboardData)
        {
            _unknownClipboardData = clipboardData;
        }

        #endregion

        #region Private fields

        private UnknownClipboardData _unknownClipboardData;

        #endregion

        #region Public methods

        public string[] GetFormats() => _unknownClipboardData.GetFormats();

        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();
            GC.SuppressFinalize(this);
        }

        ~UnknownClipboardDataViewModel()
        {
            Dispose();
        }

        #endregion

    }
}
