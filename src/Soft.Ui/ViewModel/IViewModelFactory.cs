namespace Soft.Ui.ViewModel
{
    /// <summary>
    /// Interface for a ViewModelFactory. Creates a ViewModel from a string name (nameof()) of a ViewModel class i.E. nameof(CustomerDetailViewModel).
    /// </summary>
    public interface IViewModelFactory
    {
        IViewModel Create(string viewModelname);
    }
}