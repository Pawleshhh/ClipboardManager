namespace ManiacClipboard.ViewModel
{
    public interface IUserMessageService
    {

        void InfoMessage(string text, string caption);

        void InfoMessage(string text, string caption, UserMessageAppearance appearance);

        UserMessageResult DecisionMessage(string text, string caption, UserMessageOptions options);

        UserMessageResult DecisionMessage(string text, string caption, UserMessageOptions options, UserMessageAppearance appearance);

    }
}
