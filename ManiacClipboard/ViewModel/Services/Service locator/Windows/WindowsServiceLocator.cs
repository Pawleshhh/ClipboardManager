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

        public IUserMessageService UserMessageService
        {
            get
            {
                if (_userMessageService == null)
                    _userMessageService = new UserMessageWindowsService();

                return _userMessageService;
            }
        }

        #endregion

        #region Static fields

        private static IClipboardService _clipboardService;

        private static IUserMessageService _userMessageService;

        #endregion

    }
}
