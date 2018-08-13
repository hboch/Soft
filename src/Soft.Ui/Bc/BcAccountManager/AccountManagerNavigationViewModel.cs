using Prism.Events;
using Soft.DataAccess.Shared.Lookup;
using Soft.Model;
using Soft.Ui.ViewModel;
using System.Threading.Tasks;

namespace Soft.Ui.Bc.BcAccountManager
{
    public class AccountManagerNavigationViewModel : NavigationViewModelBase<AccountManager>
    {
        /// <summary>
        /// Returns the string which will be used for the Tab Header in the UI for an Account Manager
        /// </summary>
        public override string TabDisplayName
        {
            get { return "Account Manager"; }
        }

        #region Constructor
        public AccountManagerNavigationViewModel(
            IEventAggregator eventAggregator,
            IGenericLookupDataService<AccountManager> accountManagerLookupDataService)
            : base(eventAggregator, accountManagerLookupDataService)
        {
        }
        #endregion

        #region Functions
        public override Task<bool> LoadAsync(int id)
        {
            return base.LoadAsyncBase();
        }

        /// <summary>
        /// Returns the name of the DetailViewModel
        /// </summary>
        /// <returns></returns>
        public override string NameOfDetailViewModel()
        {
            return nameof(AccountManagerDetailViewModel);
        }
        #endregion        
    }
}
