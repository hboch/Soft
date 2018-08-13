using Prism.Commands;
using Prism.Events;
using Soft.Model;
using Soft.Ui.ViewModel;
using Soft.Ui.ViewModel.Services;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Soft.Ui.Bc.BcAccountManager
{
    public class AccountManagerDetailViewModel : DetailViewModelBase
    {
        #region Properties
        private IAccountManagerRepository _repository;

        private AccountManagerWrapper _accountManagerWrapper;

        public AccountManagerWrapper AccountManagerWrapper
        {
            get { return _accountManagerWrapper; }
            private set
            {
                _accountManagerWrapper = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Returns the string which will be used for the Tab Header in the UI for an Account Manager
        /// </summary>
        public override string TabDisplayName
        {
            get
            {
                string tabDisplayName = AccountManagerWrapper.Name;
                if (HasChanges) tabDisplayName = tabDisplayName + " *";
                return tabDisplayName;
            }
        }
        #endregion

        #region Constructor
        public AccountManagerDetailViewModel(
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IAccountManagerRepository repository)
            : base(eventAggregator, messageDialogService)
        {
            _repository = repository;
        }
        #endregion
        public override async Task<bool> LoadAsync(int accountManagerId)
        {
            var accountManager = accountManagerId > 0
                ? await _repository.FindByIdAsync(accountManagerId)
                : CreateNewAccountManager();

            Id = accountManagerId;

            InitializeAccountManager(accountManager);
            return true;
        }

        private void InitializeAccountManager(AccountManager accountManager)
        {
            AccountManagerWrapper = new AccountManagerWrapper(accountManager);

            //TODO Wie testen, dass hinzugefügt wurde ?
            AccountManagerWrapper.PropertyChanged += AccountManagerWrapper_PropertyChanged;

            //TODO Wie testen, dass Event aufgerufen wurde ?
            ((DelegateCommand)CommandSave).RaiseCanExecuteChanged();
        }

        private void AccountManagerWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //TODO Wie Inhalt testen ?
            if (HasChanges == false)
            {
                HasChanges = _repository.HasChanges();
            }

            if (e.PropertyName == nameof(AccountManagerWrapper.HasErrors))
            {
                ((DelegateCommand)CommandSave).RaiseCanExecuteChanged();
            }
        }

        private AccountManager CreateNewAccountManager()
        {
            var newAccountManager = new AccountManager();

            _repository.Add(newAccountManager);

            return newAccountManager;
        }

        protected override void OnCommandDeleteExecute()
        {
            var result = base.MessageDialogService.ShowOkCancelDialog($"Do you really want to delete the Account Manager ?", "Delete");
            if (result == MessageDialogResult.OK)
            {
                _repository.Remove(AccountManagerWrapper.Entity);

                _repository.SaveAsync();

                RaiseEventAfterDetailDeleted(Id);
            }
        }

        protected override bool OnCommandSaveCanExecute()
        {
            return
                 (AccountManagerWrapper != null)
                 && (HasChanges == true)
                 && (AccountManagerWrapper.HasErrors == false);
        }

        protected override async void OnCommandSaveExecute()
        {
            await _repository.SaveAsync();

            HasChanges = _repository.HasChanges();

            RaiseEventAfterDetailSaved(Id);
            //Update the Tab Header in  the UI
            OnPropertyChanged(nameof(TabDisplayName));
        }
    }
}
