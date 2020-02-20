using System;
using System.Windows;

namespace ManiacClipboard.ViewModel
{
    public class WindowsServiceLocator : IServiceLocator
    {

        #region Constructors

        public WindowsServiceLocator(Window window)
        {
            if (window == null)
                throw new ArgumentNullException("window");

            this.window = window;
        }

        #endregion

        #region Private fields

        private readonly Window window;

        #endregion

        #region Properties

        public IClipboardService ClipboardService
        {
            get
            {
                if (_clipboardService == null)
                    _clipboardService = new WindowsClipboardService(window);

                return _clipboardService;
            }
        }

        #endregion

        #region Static

        private static IClipboardService _clipboardService;

        #endregion

    }
}
