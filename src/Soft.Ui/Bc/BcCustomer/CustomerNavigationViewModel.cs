using Prism.Events;
using Soft.DataAccess.Shared.Lookup;
using Soft.Model;
using Soft.Ui.ViewModel;
using System.Threading.Tasks;

namespace Soft.Ui.Bc.BcCustomer
{
    /// <summary>
    /// Implementation of the NavigationViewModel for a Customer Entity
    /// </summary>
    public class CustomerNavigationViewModel : NavigationViewModelBase<Customer>
    {
        #region Properties
        //Returns the name of the ViewModel, to be dislayed in the UI
        public override string TabDisplayName
        {
            get { return "Customers"; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constrctor to call the base class constructor
        /// </summary>
        /// <param name="eventAggregator"></param>
        /// <param name="customerLookupDataService"></param>
        public CustomerNavigationViewModel(IEventAggregator eventAggregator, IGenericLookupDataService<Customer> customerLookupDataService)
            : base(eventAggregator, customerLookupDataService)
        {
        }
        #endregion

        #region Functions
        /// <summary>
        /// Overrides LoadAsync to only call LoadAsyncBase 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Task<bool> LoadAsync(int id)
        {
            return base.LoadAsyncBase();
        }
        /// <summary>
        /// Returns the (Customer)DetailViewModel which belongs to the NavigationViewModel
        /// </summary>
        /// <returns></returns>
        public override string NameOfDetailViewModel()
        {
            return nameof(CustomerDetailViewModel);
        }
        #endregion
    }
}
