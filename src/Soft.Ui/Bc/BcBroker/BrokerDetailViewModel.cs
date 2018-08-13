using Prism.Commands;
using Prism.Events;
using Soft.Model;
using Soft.Ui.ViewModel;
using Soft.Ui.ViewModel.Services;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Soft.Ui.Bc.BcBroker
{
    /// <summary>
    /// DetailViewModel for a Broker Entity
    /// </summary>
    public class BrokerDetailViewModel : DetailViewModelBase, IBrokerDetailViewModel
    {
        #region Properties
        public override string TabDisplayName
        {
            get
            {
                string tabDisplayName = CustomerBrokerWrapper.Name;
                if (HasChanges) tabDisplayName = tabDisplayName + " *";
                return tabDisplayName;
            }
        }

        private IMessageDialogService _messageDialogService;
        private IBrokerRepository _repository;

        private BrokerWrapper _customerBrokerWrapper;

        public BrokerWrapper CustomerBrokerWrapper
        {
            get { return _customerBrokerWrapper; }
            private set
            {
                _customerBrokerWrapper = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor
        public BrokerDetailViewModel(IEventAggregator eventAggregator,
                                        IMessageDialogService messageDialogService,
                                        IBrokerRepository customerBrokerRepository)
            : base(eventAggregator, messageDialogService)
        {
            _messageDialogService = messageDialogService;
            _repository = customerBrokerRepository;
        }
        #endregion

        #region EventHandler
        protected override void OnCommandDeleteExecute()
        {
            var result = _messageDialogService.ShowOkCancelDialog($"Do you really want to delete the Broker ?", "Delete");
            if (result == MessageDialogResult.OK)
            {
                _repository.Remove(CustomerBrokerWrapper.Entity);
                _repository.SaveAsync();

                RaiseEventAfterDetailDeleted(Id);
            }
        }

        protected override bool OnCommandSaveCanExecute()
        {
            return
                 (CustomerBrokerWrapper != null)
                 && (HasChanges == true)
                 && (CustomerBrokerWrapper.HasErrors == false);
        }

        protected override async void OnCommandSaveExecute()
        {
            await _repository.SaveAsync();

            HasChanges = _repository.HasChanges();

            RaiseEventAfterDetailSaved(Id);
        }
        #endregion

        #region Functions
        /// <summary>
        /// Returns a new Model.Broker and adds it to the repository
        /// </summary>
        /// <returns></returns>
        private Broker CreateNewBroker()
        {
            var newBroker = new Broker();

            _repository.Add(newBroker);

            return newBroker;
        }

        /// <summary>
        /// Async Load of the ViewModel
        /// </summary>
        /// <param name="brokerId">Id of the Broker to load</param>
        /// <returns></returns>
        public override async Task<bool> LoadAsync(int brokerId)
        {

            var broker = brokerId > 0
                ? await _repository.FindByIdAsync(brokerId)
                : CreateNewBroker();

            Id = brokerId;

            InitializeBroker(broker);
            return true;
        }

        private void InitializeBroker(Broker broker)
        {
            CustomerBrokerWrapper = new BrokerWrapper(broker);

            CustomerBrokerWrapper.PropertyChanged += CustomerBrokerWrapper_PropertyChanged;

            ((DelegateCommand)CommandSave).RaiseCanExecuteChanged();
        }

        private void CustomerBrokerWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (HasChanges == false)
            {
                HasChanges = _repository.HasChanges();
            }

            if (e.PropertyName == nameof(CustomerBrokerWrapper.HasErrors))
            {
                ((DelegateCommand)CommandSave).RaiseCanExecuteChanged();
            }
        }
        #endregion
    }
}