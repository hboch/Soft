namespace Soft.Ui.ViewModel
{
    /// <summary>
    /// Interface for a MainNavigationViewModel of an application to navigate through the MainNavigationItemViewModels.
    /// </summary>
    public interface IMainNavigationViewModel
    {
        /// <summary>
        /// Load the list of MainNavigationItemViewModels to navigate through
        /// </summary>
        void Load();
    }
}
