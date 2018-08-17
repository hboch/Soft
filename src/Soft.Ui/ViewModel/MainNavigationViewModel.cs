using Prism.Events;
using Soft.Ui.Bc.BcAccountManager;
using Soft.Ui.Bc.BcBroker;
using Soft.Ui.Bc.BcCustomer;
using System.Collections.ObjectModel;

namespace Soft.Ui.ViewModel
{
    /// <summary>
    /// Implementation for IMainNavigationViewModel of an application to navigate through defined MainNavigationItemViewModels.
    /// Load function creates hardcoded a list of MainNavigationItemViewModels which is then used as a menu to navigate through Entities having a NavigationView(Model) and a DetailView(Model).
    /// Navigation through the MenuItems is implemented by using Events.
    /// </summary>
    public class MainNavigationViewModel : IMainNavigationViewModel
    {
        #region Properties
        private IEventAggregator _eventAggregator;
        public ObservableCollection<MainNavigationItemViewModel> MenuItems { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructs a MainNavigationViewModel by given Eventaggregator and new empty MenuItems list.
        /// </summary>
        /// <param name="eventAggregator">Eventaggregator to fire Navigation Events</param>
        public MainNavigationViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            MenuItems = new ObservableCollection<MainNavigationItemViewModel>();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Clears MenuItems list first and then adds (hardcoded) MainNavigationItemViewModels to the MenuItems list.
        /// </summary>
        public void Load()
        {
            //TODO Write unit test
            //TODO Dynamische Menustruktur hinzufügen
            MenuItems.Clear();
            //Add a MenuItem by using the following schema:
            //MenuItems.Add(new MainNavigationItemViewModel(displayMember: {Menu item display name in the UI}, viewModelName: nameof({Entity}NavigationViewModel), eventAggregator: _eventAggregator));
            MenuItems.Add(new MainNavigationItemViewModel(iconPackUri: "pack://application:,,,/Resources/AccountManager.png", displayMember: "Account Managers", viewModelName: nameof(AccountManagerNavigationViewModel), eventAggregator: _eventAggregator));
            MenuItems.Add(new MainNavigationItemViewModel(iconPackUri: "pack://application:,,,/Resources/Broker.png", displayMember: "Brokers", viewModelName: nameof(BrokerNavigationViewModel), eventAggregator: _eventAggregator));
            MenuItems.Add(new MainNavigationItemViewModel(iconPackUri: "pack://application:,,,/Resources/Customers.png", displayMember: "Customers", viewModelName: nameof(CustomerNavigationViewModel), eventAggregator: _eventAggregator));
        }
        #endregion
    }
}
