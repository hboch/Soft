using Prism.Commands;
using Prism.Events;
using Soft.Ui.Shared.Events;
using System.Windows.Input;

namespace Soft.Ui.ViewModel
{
    /// <summary>
    /// A (menu) item, representing an Entity, in the Entity Navigation View
    /// </summary>
    public class NavigationItemViewModel : NotifyPropertyChangedBase
    {
        #region Properties
        private string _detailViewModelName;
        private IEventAggregator _eventAggregator;
        private string _displayMember;

        public int Id { get; }

        public string DisplayMember
        {
            get { return _displayMember; }
            set
            {
                _displayMember = value;
                OnPropertyChanged();
            }
        }

        public ICommand CommandOpenDetailViewModel { get; }
        #endregion

        #region Constructor
        public NavigationItemViewModel(int id, string displayMember, string detailViewModelName, IEventAggregator eventAggregator)
        {
            _detailViewModelName = detailViewModelName;
            _eventAggregator = eventAggregator;

            Id = id;
            DisplayMember = displayMember;
            CommandOpenDetailViewModel = new DelegateCommand(OnCommandOpenDetailViewModel);
        }
        #endregion

        #region Functions
        private void OnCommandOpenDetailViewModel()
        {
            _eventAggregator.GetEvent<EventOpenNavigationOrDetailViewModel>()
                .Publish(new EventOpenNavigationOrDetailViewModelArgs
                {
                    Id = Id,
                    ViewModelName = _detailViewModelName
                });
        }
        #endregion
    }
}
