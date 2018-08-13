using System.Threading.Tasks;

namespace Soft.Ui.ViewModel
{
    /// <summary>
    /// Interface for a NavigationViewModel of a list of Entities
    /// </summary>
    public interface INavigationViewModel
    {
        /// <summary>
        /// Load the list of Entities to navigate through
        /// </summary>
        /// <returns></returns>
        Task LoadAsync();
    }
}