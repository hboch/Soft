using Prism.Events;
using Soft.DataAccess.Shared.Lookup;
using Soft.Model;
using Soft.Ui.ViewModel;
using System.Threading.Tasks;

namespace Soft.Ui.Bc.BcBroker
{
    public class BrokerNavigationViewModel : NavigationViewModelBase<Broker>
    {
        public override string TabDisplayName
        {
            get { return "Broker"; }
        }

        #region Constructor
        public BrokerNavigationViewModel(
            IEventAggregator eventAggregator,
            IGenericLookupDataService<Broker> brokerLookupDataService)
            : base(eventAggregator, brokerLookupDataService)
        {
        }
        #endregion

        #region Functions
        public override Task<bool> LoadAsync(int id)
        {
            return base.LoadAsyncBase();
        }
        public override string NameOfDetailViewModel()
        {
            return nameof(BrokerDetailViewModel);
        }
        #endregion        
    }
}

