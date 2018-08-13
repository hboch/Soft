using System.Threading.Tasks;
using System.Windows.Input;

namespace Soft.Ui.ViewModel
{
    /// <summary>
    /// ViewModel Interface for DetailViewModels and NavigationViewModels
    /// </summary>
    public interface IViewModel
    {
        //In combination with the ViewModel Name a unique ViewModel Id
        int Id { get; }

        //ViewModel Name 
        string Name { get; }

        //ViewModel DisplayName as displayed in the UI
        string TabDisplayName { get; }

        //Viewmodel Close command
        ICommand CommandClose { get; }

        /// <summary>
        /// Async Load function to load the ViewModel content 
        /// </summary>
        /// <param name="id">In case of a DetailViewModel the Id is the Entity-Id</param>
        /// <returns></returns>
        Task<bool> LoadAsync(int id);
    }
}
