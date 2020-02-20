
using System;
using System.Windows;

namespace ManiacClipboard.ViewModel
{
    public class UserMessageWindowsService : IUserMessageService
    {

        #region Public methods

        public UserMessageResult DecisionMessage(string text, string caption, UserMessageOptions options)
        {
            return GetUserMessageResult(MessageBox.Show(text, caption, OptionsToButtons(options)));
        }

        public UserMessageResult DecisionMessage(string text, string caption, UserMessageOptions options, UserMessageAppearance appearance)
        {
            return GetUserMessageResult(MessageBox.Show(text, caption, OptionsToButtons(options), GetMessageBoxImage(appearance)));
        }

        public void InfoMessage(string text, string caption)
        {
            MessageBox.Show(text, caption);
        }

        public void InfoMessage(string text, string caption, UserMessageAppearance appearance)
        {
            MessageBox.Show(text, caption, MessageBoxButton.OK, GetMessageBoxImage(appearance));
        }

        #endregion

        #region Private methods

        private MessageBoxButton OptionsToButtons(UserMessageOptions options)
        {
            switch(options)
            {
                case UserMessageOptions.Ok:
                    return MessageBoxButton.OK;
                case UserMessageOptions.OkCancel:
                    return MessageBoxButton.OKCancel;
                case UserMessageOptions.YesNo:
                    return MessageBoxButton.YesNo;
                case UserMessageOptions.YesNoCancel:
                    return MessageBoxButton.YesNoCancel;
                default:
                    throw new ArgumentException("Undefined options", "options");
            }
        }

        private UserMessageResult GetUserMessageResult(MessageBoxResult result)
        {
            switch(result)
            {
                case MessageBoxResult.OK:
                    return UserMessageResult.Ok;
                case MessageBoxResult.Yes:
                    return UserMessageResult.Yes;
                case MessageBoxResult.No:
                    return UserMessageResult.No;
                case MessageBoxResult.Cancel:
                    return UserMessageResult.Cancel;
                default:
                    throw new ArgumentException("Undefined result");
            }
        }

        private MessageBoxImage GetMessageBoxImage(UserMessageAppearance appearance)
        {
            switch(appearance)
            {
                case UserMessageAppearance.Error:
                    return MessageBoxImage.Error;
                case UserMessageAppearance.Info:
                    return MessageBoxImage.Information;
                case UserMessageAppearance.Question:
                    return MessageBoxImage.Question;
                case UserMessageAppearance.Warning:
                    return MessageBoxImage.Warning;
                default:
                    throw new ArgumentException("Undefined appearance", "appearance");
            }
        }

        #endregion
    }
}
