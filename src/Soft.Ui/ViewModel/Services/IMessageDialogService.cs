namespace Soft.Ui.ViewModel.Services
{
    /// <summary>
    /// Interface for a dialog window
    /// </summary>
    public interface IMessageDialogService
    {
        MessageDialogResult ShowOkCancelDialog(string text, string title);
        void ShowInfoDialog(string text, string title);
    }
}