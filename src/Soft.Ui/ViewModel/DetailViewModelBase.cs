using Prism.Commands;
using Prism.Events;
using Soft.Ui.Shared.Events;
using Soft.Ui.ViewModel.Services;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Soft.Ui.ViewModel
{
    /// <summary>
    /// Abstract base class for a DetailViewModel of an Entity
    /// </summary>
    public abstract class DetailViewModelBase : NotifyPropertyChangedBase, IDetailViewModel
    {
        #region Properties
        public abstract string TabDisplayName { get; }
        protected readonly IEventAggregator EventAggregator;
        protected readonly IMessageDialogService MessageDialogService;

        public ICommand CommandSave { get; set; }
        public ICommand CommandDelete { get; set; }
        public ICommand CommandClose { get; }

        private bool _hasChanges;
        /// <summary>
        /// Has data in the Viewmodel changed
        /// </summary>
        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if (value != HasChanges)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TabDisplayName));

                    //ToDo Test für RaiseCanExecuteChanged()
                    ((DelegateCommand)CommandSave).RaiseCanExecuteChanged();
                }
            }
        }
        private int _id;
        /// <summary>
        /// In combination with the ViewModel Name a unique ViewModel Id. For an existing Entity is ViewModel-Id = Entity-Id.
        /// </summary>
        public int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }

        /// <summary>
        /// ViewModel Name 
        /// </summary>
        public string Name
        {
            get { return this.GetType().Name; }
        }
        #endregion

        #region Constructor
        public DetailViewModelBase(IEventAggregator eventAggregator, IMessageDialogService messageDialogService)
        {
            EventAggregator = eventAggregator;
            MessageDialogService = messageDialogService;

            CommandSave = new DelegateCommand(OnCommandSaveExecute, OnCommandSaveCanExecute);
            CommandDelete = new DelegateCommand(OnCommandDeleteExecute);
            CommandClose = new DelegateCommand(OnCommandCloseExecute);
        }
        #endregion

        #region EventHandler
        protected abstract void OnCommandDeleteExecute();
        protected abstract bool OnCommandSaveCanExecute();
        protected abstract void OnCommandSaveExecute();
        protected virtual void OnCommandCloseExecute()
        {
            if (HasChanges)
            {
                var result = MessageDialogService
                    .ShowOkCancelDialog("Data has changed. Close without saving ?", "Closing");
                if (result == MessageDialogResult.Cancel)
                {
                    return;
                }
            }
            RaiseEventAfterDetailClose();
        }

        protected virtual void RaiseEventAfterDetailDeleted(int modelId)
        {
            EventAggregator.GetEvent<EventAfterDetailDeleted>().Publish(
                new EventAfterDetailDeletedArgs { Id = modelId, ViewModelName = this.GetType().Name });
        }
        protected virtual void RaiseEventAfterDetailSaved(int modelId)
        {
            EventAggregator.GetEvent<EventAfterDetailSaved>().Publish(
                new EventAfterDetailSavedArgs
                {
                    Id = modelId,
                    ViewModelName = this.GetType().Name
                });
        }
        protected virtual void RaiseEventAfterDetailClose()
        {
            EventAggregator.GetEvent<EventAfterDetailClose>().Publish(
                new EventAfterDetailCloseArgs { Id = this.Id, ViewModelName = this.GetType().Name });
        }
        #endregion

        #region Functions
        public abstract Task<bool> LoadAsync(int id);
        #endregion
    }
}
