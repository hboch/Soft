using Prism.Commands;
using Prism.Events;
using Soft.DataAccess.Shared.Lookup;
using Soft.Model.Shared;
using Soft.Ui.Shared.Events;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Soft.Ui.ViewModel
{
    /// <summary>
    /// Abstract base class for a NavigationViewModel.
    /// A NavigationViewModel is used to display a list of (Entities) items of type NavigationItemViewModel.
    /// A click on a NavigationItem opens the DetailViewModel of the (Entity) item <see>NavigationItemViewModel</see>.
    /// The list of items are created by the constructor parameter entityLookupDataService.
    /// The function entityLookupDataService.GetEntityLookupAsync() returns a list of TEntity which are then 
    /// used to create a list of items of type NavigationItemViewModel.
    /// This class also implements the Close command (when a NavigationView has to be closed).
    /// This class also reacts on Saved and Deleted events of the created DetailViewmodels.
    /// </summary>
    /// <typeparam name="TEntity">Navigation Items are of type TEntity</typeparam>
    public abstract class NavigationViewModelBase<TEntity> : NotifyPropertyChangedBase, IViewModel
        where TEntity : EntityBase
    {
        #region Properties
        //In the constructor given EventAggregator, to react on DetailViewModel events
        protected readonly IEventAggregator EventAggregator;

        //Close Command of NavigationView
        public ICommand CommandClose { get; }

        //Command to create a {Entity}DetailViewModel for a new none existing Entity
        public ICommand CommandCreateNewDetail { get; set; }

        private int _id;
        /// <summary>
        /// Id of the NavigationViewModel
        /// </summary>
        public int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }

        /// <summary>
        /// Name of the NavigationViewModel
        /// </summary>
        public string Name { get { return this.GetType().Name; } }

        //In the constructor given Entity LookupDataService. 
        //For each Entity returned by the LookupDataService a corresponding Entity NavigationItemViewModel will be created.
        private readonly IGenericLookupDataService<TEntity> _entityLookupDataService;

        // List of Entity NavigationItemViewModels
        public ObservableCollection<NavigationItemViewModel> EntityNavigationItemViewModels { get; private set; }

        /// <summary>
        /// Returns the display name, which will be displayed in the Tab-Header for NavigationViewModel in the UI.
        /// </summary>
        public abstract string TabDisplayName { get; }

        /// <summary>
        /// HasChanges is always false for a NavigationViewModel, as it can not be saved.
        /// </summary>
        public bool HasChanges { get { return false; } }
        #endregion

        #region Constructor
        public NavigationViewModelBase(IEventAggregator eventAggregator, IGenericLookupDataService<TEntity> entityLookupDataService)
        {
            //Keep constructor parameters
            _entityLookupDataService = entityLookupDataService;
            EntityNavigationItemViewModels = new ObservableCollection<NavigationItemViewModel>();
            EventAggregator = eventAggregator;

            //Delegate Commands
            CommandClose = new DelegateCommand(OnCommandClose);
            CommandCreateNewDetail = new DelegateCommand(OnCommandCreateNewDetail);

            //Subscribe to DetailViewModel events 
            EventAggregator.GetEvent<EventAfterDetailSaved>().Subscribe(OnEventAfterDetailSaved);
            EventAggregator.GetEvent<EventAfterDetailDeleted>().Subscribe(OnEventAfterDetailDeleted);
        }
        #endregion

        #region CommandHandler
        /// <summary>
        /// Fire EventAfterDetailClose when View was closed
        /// </summary>
        private void OnCommandClose()
        {
            EventAggregator.GetEvent<EventAfterDetailClose>().Publish(
                   new EventAfterDetailCloseArgs { Id = this.Id, ViewModelName = this.GetType().Name });
        }

        private int nextNewItemId = 0;
        /// <summary>
        /// Fire EventOpenNavigationOrDetailViewModel when creating a Entity DetailViewModel for a new none existing Entity.
        /// </summary>
        /// <remarks>EventOpenNavigationOrDetailViewModelArgs.Id is decremented by each call, to allow more than one new Entity. 
        /// A negative Id identifies a new not yet saved Entity</remarks>
        private void OnCommandCreateNewDetail()
        {
            EventAggregator.GetEvent<EventOpenNavigationOrDetailViewModel>().Publish(
                new EventOpenNavigationOrDetailViewModelArgs { Id = nextNewItemId--, ViewModelName = NameOfDetailViewModel() });
        }
        #endregion 

        #region EventHandler
        /// <summary>
        /// Reload NavigationItems, when a Saved event in a created Entity DetailViewModel was fired
        /// as the display name, which will be displayed in the Tab-Header for NavigationViewModel in the UI might depend on Entity properties.
        /// </summary>
        /// <param name="eventAfterDetailSavedArgs"></param>
        public void OnEventAfterDetailSaved(EventAfterDetailSavedArgs eventAfterDetailSavedArgs)
        {
            if (eventAfterDetailSavedArgs.ViewModelName == NameOfDetailViewModel())
            {
                ReloadAsync();
            }
        }
        /// <summary>
        /// Remove item from EntityNavigationItemViewModels list, 
        /// when a Deleted event in a created Entity DetailViewModel was fired
        /// </summary>
        /// <param name="eventAfterDetailDeletedArgs"></param>
        public void OnEventAfterDetailDeleted(EventAfterDetailDeletedArgs eventAfterDetailDeletedArgs)
        {
            if (eventAfterDetailDeletedArgs.ViewModelName == NameOfDetailViewModel())
            {
                var entityNavigationItemViewModel = EntityNavigationItemViewModels.SingleOrDefault(k => k.Id == eventAfterDetailDeletedArgs.Id);
                if (entityNavigationItemViewModel != null)
                {
                    EntityNavigationItemViewModels.Remove(entityNavigationItemViewModel);
                }
            }
        }
        #endregion

        #region Functions
        /// <summary>
        /// Creates a list of NavigationItemViewModel representing the Entities to navigate through.
        /// </summary>
        /// <returns>True, when craetion was succesfull</returns>
        /// <remarks>
        /// The list of items are created by using the constructor parameter entityLookupDataService.
        /// The function entityLookupDataService.GetEntityLookupAsync() returns a list of TEntity. 
        /// From TEntity.Id (unique identifier of a TEntity), 
        /// TEntity.DisplayMember (string representing the display member of TEntity in the UI), 
        /// NameOfDetailViewModel() (Name of the DetailViewModel in which TEntity will be displyed in the UI)
        /// and the constructor EventAggregator parameter
        /// a list of items of type NavigationItemViewModel will be created.
        /// </remarks>
        protected async Task<bool> LoadAsyncBase()
        {
            //TODO Write unit test
            //TODO Write integration test
            var lookupItems = await _entityLookupDataService.GetEntityLookupAsync();

            EntityNavigationItemViewModels.Clear();
            foreach (var lookupItem in lookupItems)
            {
                EntityNavigationItemViewModels.Add(
                    new NavigationItemViewModel
                    (
                        lookupItem.Id,
                        lookupItem.DisplayMember,
                        NameOfDetailViewModel(),
                        EventAggregator)
                    );
            }
            return true;
        }
        /// <summary>
        /// Reload NavigationViewModel content
        /// </summary>
        private async void ReloadAsync()
        {
            // id is always 0 for a NavigationViewModel
            await LoadAsync(id: 0);
        }

        /// <summary>
        /// Load ViewModel content
        /// </summary>
        /// <param name="id">is always 0 for a NavigationViewModel . It is a Dummy-Parameter to have the same signature as fopr a DetailViewModel.</param>
        /// <returns>True, if Load was successful</returns>
        public abstract Task<bool> LoadAsync(int id);

        /// <summary>
        /// Return the name of the DetailViewModel correponding to the NavigationViewModel
        /// </summary>
        /// <returns></returns>
        public abstract string NameOfDetailViewModel();
        #endregion
    }
}
