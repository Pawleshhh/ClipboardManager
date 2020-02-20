using System;

namespace ManiacClipboard.ViewModel
{
    public class MainViewModel : NotifyPropertyChanges
    {

        #region Constructors

        public MainViewModel(IServiceLocator serviceLocator)
        {
            if (serviceLocator == null)
                throw new ArgumentNullException("serviceLocator");

            ClipboardManager = new ClipboardManagerViewModel(serviceLocator.GetClipboardService());
        }

        #endregion

        #region Private fields

        private string _title;

        #endregion

        #region Properties

        public ClipboardManagerViewModel ClipboardManager { get; }

        public string Title
        {
            get => _title;
            set => SetProperty(() => _title == value, () => _title = value);
        }

        #endregion

        #region Public methods

        #endregion

        #region Private methods

        protected override void DisposeUnmanaged()
        {
            ClipboardManager.Dispose();
        }

        #endregion

    }
}
