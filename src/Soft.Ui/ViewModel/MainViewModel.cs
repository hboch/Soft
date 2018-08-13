using Prism.Commands;
using Prism.Events;
using Soft.Ui.Shared.Events;
using Soft.Ui.ViewModel.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Soft.Ui.ViewModel
{
    public class MainViewModel : NotifyPropertyChangedBase
    {
        #region Properties
        public IMainNavigationViewModel MainNavigationViewModel { get; }
        private IViewModelFactory _viewModelFactory;

        private IEventAggregator _eventAggregator;
        private IMessageDialogService _messageDialogService;

        public ObservableCollection<IViewModel> ViewModels { get; }

        private IViewModel _selectedViewModel;
        public IViewModel SelectedViewModel
        {
            get { return _selectedViewModel; }
            set
            {
                _selectedViewModel = value;
                OnPropertyChanged();
            }
        }
        public ICommand CommandCreateSingleDetailView { get; set; }
        #endregion

        #region Constructor
        public MainViewModel
            (
            IMainNavigationViewModel mainNavigationViewModel,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IViewModelFactory viewModelFactory
            )
        {
            MainNavigationViewModel = mainNavigationViewModel;
            _viewModelFactory = viewModelFactory;
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;

            _eventAggregator.GetEvent<EventOpenNavigationOrDetailViewModel>().Subscribe(OnEventOpenNavigationOrDetailViewModel);
            _eventAggregator.GetEvent<EventAfterDetailDeleted>().Subscribe(OnEventAfterDetailDeleted);
            _eventAggregator.GetEvent<EventAfterDetailClose>().Subscribe(OnEventAfterDetailClose);

            ViewModels = new ObservableCollection<IViewModel>();

            CommandCreateSingleDetailView = new DelegateCommand<Type>(OnCommandCreateSingleDetailView);
        }
        #endregion

        #region EventHandler
        /// <summary>
        /// If the ViewModel, identified by the combination of the event arguments, not already exists in the ViewModels list,
        /// then a new Viewmodel (NavigationViewModel or DetailViewModel) is created from the event arguments,
        /// and LoadAsync function of the created ViewModel is called
        /// and ViewModel is added tio ViewModels list
        /// and SelectedViewModel is set to ViewModel.
        /// </summary>
        /// <param name="eventOpenViewModelArgs">Id and Name of the ViewModel</param>
        private async void OnEventOpenNavigationOrDetailViewModel(EventOpenNavigationOrDetailViewModelArgs eventOpenViewModelArgs)
        {
            var viewModel = ViewModels.SingleOrDefault(dvm
                => dvm.Id == eventOpenViewModelArgs.Id
                && dvm.Name == eventOpenViewModelArgs.ViewModelName);

            //ViewModel does not exist ViewModels
            //-> create, call ViewModels LoadAsync function and add ViewModel to ViewModels
            if (viewModel == null)
            {
                viewModel = _viewModelFactory.Create(eventOpenViewModelArgs.ViewModelName);
                bool loadAsyncSucessful = await viewModel.LoadAsync(eventOpenViewModelArgs.Id);
                if (!loadAsyncSucessful)
                {
                    _messageDialogService.ShowInfoDialog("Entry could not be loaded as it might have been deleted. Displayed Entries are refreshed.", "Information");
                    //await NavigationViewModel.LoadAsync();
                    return;
                }
                ViewModels.Add(viewModel);
            }
            SelectedViewModel = viewModel;
        }
        /// <summary>
        /// Removes ViewModel, identified by the combination of the event arguments, from ViewModels list, after Detail (Entity) was deleted.
        /// </summary>
        /// <param name="eventAfterDetailDeletedArgs">Id and Name of the ViewModel</param>
        private void OnEventAfterDetailDeleted(EventAfterDetailDeletedArgs eventAfterDetailDeletedArgs)
        {
            RemoveViewModel(eventAfterDetailDeletedArgs.Id, eventAfterDetailDeletedArgs.ViewModelName);
        }

        /// <summary>
        /// Removes ViewModel, identified by the combination of the event arguments, from ViewModels list, after DetailView (Entity) was closed.
        /// </summary>
        /// <param name="eventAfterDetailCloseArgs">Id and Name of the ViewModel</param>
        private void OnEventAfterDetailClose(EventAfterDetailCloseArgs eventAfterDetailCloseArgs)
        {
            RemoveViewModel(eventAfterDetailCloseArgs.Id, eventAfterDetailCloseArgs.ViewModelName);
        }
        /// <summary>
        /// Removes ViewModel with given Id and ViewModelNamen from ViewModels list, if in the list.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="viewModelName"></param>
        private void RemoveViewModel(int id, string viewModelName)
        {
            var viewModel = ViewModels.SingleOrDefault(dvm => dvm.Id == id && dvm.Name == viewModelName);

            //If ViewModel is in the list
            if (viewModel != null)
            {
                ViewModels.Remove(viewModel);
            }
        }

        /// <summary>
        /// Opens ViewModel, given by a ViewModel type as command parameter, by publishing an EventOpenNavigationOrDetailViewModel.
        /// </summary>
        /// <param name="objektType">ViewModel type</param>
        private void OnCommandCreateSingleDetailView(Type objektType)
        {              
            if (objektType.GetInterfaces().Contains(typeof(IViewModel)))            
                {
                var viewModelName = objektType.Name;
                _eventAggregator.GetEvent<EventOpenNavigationOrDetailViewModel>().Publish(
                    new EventOpenNavigationOrDetailViewModelArgs { Id = 0, ViewModelName = viewModelName });
            }
        }
        #endregion

        #region Functions
        /// <summary>
        /// Calls Load function of MainNavigationViewModel
        /// </summary>
        /// <returns></returns>
        public void Load()
        {
            MainNavigationViewModel.Load();
        }
        #endregion
    }
}
