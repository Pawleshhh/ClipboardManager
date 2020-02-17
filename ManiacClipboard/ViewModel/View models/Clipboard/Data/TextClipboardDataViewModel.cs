
using ManiacClipboard.Model;

namespace ManiacClipboard.ViewModel
{
    public class TextClipboardDataViewModel : ClipboardDataViewModel<string>
    {

        #region Constructors

        public TextClipboardDataViewModel(TextClipboardData clipboardData) : base(clipboardData)
        {
        }

        #endregion

    }
}
