namespace FriendOrganizer.UI.View.Services
{
    public interface IMessageDialoagService
    {
        MessageDialogResult ShowOkCancelDialog(string text, string title);
    }
}