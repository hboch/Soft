using System.Windows;

namespace Soft.Ui.ViewModel.Services
{
    /// <summary>
    /// Implementation of IMessageDialogService using Windows Messagebox
    /// </summary>
    public class MessageDialogService : IMessageDialogService
    {
        public MessageDialogResult ShowOkCancelDialog(string text, string title)
        {
            var result = MessageBox.Show(text, title, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                return MessageDialogResult.OK;
            }
            else
            {
                return MessageDialogResult.Cancel;
            }
        }
        public void ShowInfoDialog(string text, string title)
        {
            MessageBox.Show(text, title, MessageBoxButton.OK);
        }
    }

    public enum MessageDialogResult
    {
        OK,
        Cancel
    }
}
