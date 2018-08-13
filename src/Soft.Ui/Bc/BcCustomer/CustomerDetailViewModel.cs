using Prism.Commands;
using Prism.Events;
using Soft.DataAccess.Shared.Lookup;
using Soft.Model;
using Soft.Model.Shared;
using Soft.Ui.Bc.BcAccountManager;
using Soft.Ui.Shared.Events;
using Soft.Ui.ViewModel;
using Soft.Ui.ViewModel.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Soft.Ui.Bc.BcCustomer
{
    /// <summary>
    /// DetailViewModel for a Customer Entity
    /// </summary>
    public class CustomerDetailViewModel : DetailViewModelBase, ICustomerDetailViewModel
    {
        #region Properties
        #region CommonDetailViewModelProperties
        /// <summary>
        /// Returns the string which will be used for the Tab Header in the UI for a Customer
        /// </summary>
        public override string TabDisplayName
        {
            get
            {
                string tabDisplayName = Customer.Name;
                if (HasChanges) tabDisplayName = tabDisplayName + " *";
                return tabDisplayName;
            }
        }

        /// <summary>
        /// Customer Repository
        /// </summary>
        private ICustomerRepository _repository;

        /// <summary>
        /// ViewModel Wrapper for a Customer Entity
        /// </summary>
        private CustomerWrapper _customer;
        public CustomerWrapper Customer
        {
            get { return _customer; }
            private set
            {
                _customer = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region AccountManagerProperties
        /// <summary>
        /// Lookup Service to generate a list of Account Managers to be displayedin the uI i.e. in a Combobox
        /// </summary>
        private IGenericLookupDataService<AccountManager> _lookupDataServiceAccountManager;
        public ObservableCollection<LookupItem> LookupItemsAccountManager { get; private set; }
        #endregion

        #region BankAccountProperties
        /// <summary>
        /// Command to add a bank account
        /// </summary>
        public ICommand CommandAddBankAccount { get; set; }

        /// <summary>
        /// Command to delete a bank account
        /// </summary>
        public ICommand CommandDeleteBankAccount { get; set; }

        public ObservableCollection<CustomerBankAccountWrapper> ListCustomerBankAccountWrapper { get; }
        private CustomerBankAccountWrapper _selectedAccount;
        public CustomerBankAccountWrapper SelectedBankAccount
        {
            get { return _selectedAccount; }
            set
            {
                _selectedAccount = value;
                OnPropertyChanged();
                ((DelegateCommand)CommandDeleteBankAccount).RaiseCanExecuteChanged();
            }
        }
        #endregion
        #endregion

        #region Constructor
        public CustomerDetailViewModel(ICustomerRepository customerRepository,
                                    IEventAggregator eventAggregator,
                                    IMessageDialogService messageDialogService,
                                    IGenericLookupDataService<AccountManager> accountManagerLookupDataService)
            : base(eventAggregator, messageDialogService)
        {
            _repository = customerRepository;
            _lookupDataServiceAccountManager = accountManagerLookupDataService;

            CommandDeleteBankAccount = new DelegateCommand(OnCommandDeleteAccount, OnCommandDeleteCanExecuteAccount);
            CommandAddBankAccount = new DelegateCommand(OnCommandAddAccount);

            LookupItemsAccountManager = new ObservableCollection<LookupItem>();
            ListCustomerBankAccountWrapper = new ObservableCollection<CustomerBankAccountWrapper>();

            eventAggregator.GetEvent<EventAfterDetailSaved>().Subscribe(OnEventAfterDetailSavedAsync);
        }
        #endregion

        #region EventHandler
        /// <summary>
        /// Can Customer Save Command be executed
        /// </summary>
        /// <returns>True, wenn ein Customer existiert, Änderungen aufweist, valide ist 
        /// und evtl. Marginkonten Eingaben valide sind</returns>
        protected override bool OnCommandSaveCanExecute()
        {
            return
                (Customer != null)
                && (HasChanges == true)
                && (Customer.HasErrors == false)

                && ListCustomerBankAccountWrapper.All(pn => pn.HasErrors == false);
        }

        /// <summary>
        /// Save Customer Entity repository and fire EventAfterDetailSaved
        /// </summary>
        /// <exception cref="DbUpdateConcurrencyException">Customer data can not be saved, as data was changed in the meanwhile !</exception>
        protected override async void OnCommandSaveExecute()
        {
            try
            {
                await _repository.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                MessageDialogService.ShowInfoDialog("Customer data can not be saved, as data was changed in the meanwhile !", "Save failed");
                return;
            }

            HasChanges = false;// _repository.HasChanges();

            //after saving the (new) Customer set ViewModel-Id to Customer-Id and fire event
            Id = Customer.Id;
            RaiseEventAfterDetailSaved(Id);

            //Update Tab-Header in the UI, as display name might depend on other properties
            OnPropertyChanged(nameof(TabDisplayName));
        }

        /// <summary>
        /// After user confirmation delete Customer Entity from repository and save repository and fire EventAfterDetailDeleted 
        /// </summary>
        protected override void OnCommandDeleteExecute()
        {
            var result = MessageDialogService.ShowOkCancelDialog($"Do you really want to delete the Customer ?", "Delete");
            if (result == MessageDialogResult.OK)
            {
                _repository.Remove(Customer.Entity);

                _repository.SaveAsync();

                RaiseEventAfterDetailDeleted(Id);
            }
        }

        /// <summary>
        /// Can Bank Account Delete Command be executed
        /// </summary>
        /// <returns>True, wenn ein Konto selektiert ist</returns>
        private bool OnCommandDeleteCanExecuteAccount()
        {
            return (SelectedBankAccount != null);
        }

        /// <summary>
        /// After user confirmation delete Bank Account Entity from Customer repository and list of Bank Accounts. Check if repositoryhas changes.
        /// </summary>
        private void OnCommandDeleteAccount()
        {
            var result = MessageDialogService.ShowOkCancelDialog($"Do you really want to delete the Bank Account ?", "Delete");
            if (result == MessageDialogResult.OK)
            {
                SelectedBankAccount.PropertyChanged -= CustomerAccountWrapper_PropertyChanged;

                _repository.RemoveBankAccount(SelectedBankAccount.Entity);

                ListCustomerBankAccountWrapper.Remove(SelectedBankAccount);
                SelectedBankAccount = null;

                HasChanges = _repository.HasChanges();

                //Lt. Video:
                //((DelegateCommand)CommandSave).RaiseCanExecuteChanged();
            }
        }

        private void OnCommandAddAccount()
        {
            var newCustomerAccountWrapper = new CustomerBankAccountWrapper(new BankAccount());
            newCustomerAccountWrapper.PropertyChanged += CustomerAccountWrapper_PropertyChanged;
            ListCustomerBankAccountWrapper.Add(newCustomerAccountWrapper);

            Customer.Entity.BankAccounts.Add(newCustomerAccountWrapper.Entity);

            //Trigger Validation
            newCustomerAccountWrapper.AccountNo = "";
        }

        private async void OnEventAfterDetailSavedAsync(EventAfterDetailSavedArgs eventAfterDetailSavedArgs)
        {
            await ClassifyDetailViewModel(eventAfterDetailSavedArgs.ViewModelName,
                async () =>
                {
                    LookupItemsAccountManager = await ObservableCollectionWithNullLookupItemAndLookupItems(
                          async () => await _lookupDataServiceAccountManager.GetEntityLookupAsync());
                });
        }

        private async Task ClassifyDetailViewModel(string viewModelName, Func<Task> OnAccountManagerDetailViewModel)
        {
            switch (viewModelName)
            {
                case nameof(AccountManagerDetailViewModel):
                    await OnAccountManagerDetailViewModel();
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region Functions
        /// <summary>
        /// Returns a new Model.Customer with CustomerSince=Now and adds it to the repository
        /// </summary>
        /// <returns></returns>
        private Customer CreateNewCustomer()
        {
            var customer = new Customer();
            _repository.Add(customer);

            //Set defaults for new Customer:
            customer.CustomerSince = DateTime.Now;

            return customer;
        }

        /// <summary>
        /// Returns ObservableCollection of LookupItems including a NullLookupItem (DisplaMember="-") and LookupItems returned by <param name="funcGetLookupItemsAsync">
        /// </summary>
        /// <param name="funcGetLookupItemsAsync">Async-Function which returns LookupItems</param>
        /// <returns>ObservableCollection<LookupItem></returns>
        public async Task<ObservableCollection<LookupItem>> ObservableCollectionWithNullLookupItemAndLookupItems(Func<Task<IEnumerable<LookupItem>>> funcGetLookupItemsAsync)
        {
            //Guard-Funktion
            if (funcGetLookupItemsAsync == null) throw new NullReferenceException();
            
            ObservableCollection<LookupItem> observableCollection = new ObservableCollection<LookupItem>();
            //Create NullLookupItem and add to collection
            observableCollection.Add(new NullLookupItem { DisplayMember = "-" });

            //get further LookupItems and add them to the collection
            IEnumerable<LookupItem> lookupItems = await funcGetLookupItemsAsync();
            foreach (var lookupItem in lookupItems)
            {
                observableCollection.Add(lookupItem);
            }
            return observableCollection;
        }

        /// <summary>
        /// Async Load of the ViewModel
        /// </summary>
        /// <param name="customerId">Id of the Customer to load</param>
        /// <returns></returns>
        public override async Task<bool> LoadAsync(int customerId)
        {
            Model.Customer customer;

            if (customerId > 0)
            {
                customer = await _repository.FindByIdAsync(customerId);
                if (customer == null)
                {
                    return false;
                }
            }
            else
            {
                customer = CreateNewCustomer();
            };

            Id = customerId;

            InitializeCustomer(customer);
            TriggerValidationWhenNewCustomerWrapper(Customer);

            InitializeListCustomerAccountWrapper(customer);

            LookupItemsAccountManager = await ObservableCollectionWithNullLookupItemAndLookupItems(async () => await _lookupDataServiceAccountManager.GetEntityLookupAsync());
            return true;
        }

        //TODO Refresh ggf. abhängige DetailModels z.B. AccountManager wenn diese in einem anderen Tab geändert wurden
        private void InitializeCustomer(Customer customer)
        {
            Customer = new CustomerWrapper(customer);

            Customer.PropertyChanged += Customer_PropertyChanged;

            ((DelegateCommand)CommandSave).RaiseCanExecuteChanged();
        }
        private void Customer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (HasChanges == false)
            {
                HasChanges = _repository.HasChanges();
            }

            if (e.PropertyName == nameof(Customer.HasErrors))
            {
                ((DelegateCommand)CommandSave).RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Trigger validation for a new Customer by setting the Customer Name and the CustomerNo properties to Empty
        /// </summary>
        private void TriggerValidationWhenNewCustomerWrapper(CustomerWrapper customer)
        {
            if (customer.Id == 0)
            {
                customer.Name = "";
                customer.CustomerNo = "";
            }
        }
        private void InitializeListCustomerAccountWrapper(Customer customer)
        {
            foreach (var customerAccountWrapper in ListCustomerBankAccountWrapper)
            {
                customerAccountWrapper.PropertyChanged -= CustomerAccountWrapper_PropertyChanged;
            }
            ListCustomerBankAccountWrapper.Clear();
            foreach (var customerBankAccount in customer.BankAccounts)
            {
                var customerAccountWrapper = new CustomerBankAccountWrapper(customerBankAccount);
                ListCustomerBankAccountWrapper.Add(customerAccountWrapper);
                customerAccountWrapper.PropertyChanged += CustomerAccountWrapper_PropertyChanged;
            }
        }

        private void CustomerAccountWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (HasChanges == false)
            {
                HasChanges = _repository.HasChanges();
            }

            if (e.PropertyName == nameof(CustomerBankAccountWrapper.HasErrors))
            {
                ((DelegateCommand)CommandSave).RaiseCanExecuteChanged();
            }
        }
        #endregion
    }
}
