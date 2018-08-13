namespace Soft.Ui.ViewModel
{
    /// <summary>
    /// Implementation for a IViewModelFactory. Creates a ViewModel from a string name (nameof()) of a ViewModel class i.E. nameof(CustomerDetailViewModel).
    /// Implemented by using "Autofac.Features.Indexed.IIndex<string, IViewModel>" and the Keyed-Feature of Autofac in Bootstrapper.cs.
    /// </summary>
    public class ViewModelFactory : IViewModelFactory
    {
        private Autofac.Features.Indexed.IIndex<string, IViewModel> _viewModelCreator;
        public ViewModelFactory(Autofac.Features.Indexed.IIndex<string, IViewModel> viewModelCreator)
        {
            _viewModelCreator = viewModelCreator;
        }
        public IViewModel Create(string viewModelname)
        {
            return _viewModelCreator[viewModelname];
        }
    }
}
