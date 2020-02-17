
using ManiacClipboard.Model;
using System.Collections.Generic;

namespace ManiacClipboard.ViewModel
{
    public abstract class CollectionClipboardDataViewModel<T> : ClipboardDataViewModel<IReadOnlyCollection<T>>
    {

        #region Constructors

        protected CollectionClipboardDataViewModel(CollectionClipboardData<T> clipboardData) : base(clipboardData)
        {
        }

        #endregion

        #region Public methods

        public abstract ClipboardDataViewModel[] Split();

        #endregion

    }
}
