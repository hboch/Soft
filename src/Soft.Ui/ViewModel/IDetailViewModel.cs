namespace Soft.Ui.ViewModel
{
    /// <summary>
    /// Interface for a DetailViewModel of an Entity
    /// </summary>
    public interface IDetailViewModel : IViewModel
    {
        //Has data in the Viewmodel changed
        bool HasChanges { get; }
    }
}