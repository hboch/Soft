using Prism.Commands;
using System.Windows.Input;
using System;
using Prism.Events;
using Soft.Ui.Shared.Events;

namespace Soft.Ui.ViewModel
{
    /// <summary>
    /// Class for a NavigationItemViewModel used in MainNavigationViewModel. 
    /// With the following relationship:
    /// MainNavigationViewModel -1:n- MainNavigationItemViewModel -1:1- {Entity}NavigationViewModel -1:n- {Entity}NavigationItemViewModel -1:1- {Entity}DetailViewModel
    /// One MainNavigationItemViewModel represents exactly one {Entity}NavigationViewModel
    /// </summary>
    public class MainNavigationItemViewModel
    {
        #region Properties
        private IEventAggregator _eventAggregator;
        public ICommand CommandOpenNavigationViewModel { get; }

        /// <summary>
        /// Pack URI for the Icon to be displayed
        /// </summary>
        /// <remarks>see https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-4.0/aa970069(v=vs.100) </remarks>
        public string IconPackUri { get; }

        public string DisplayMember { get; }
        public string ViewModelName { get; }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a MainNavigationItemViewModel based on a NavigationViewModel
        /// </summary>
        /// <param name="iconPackUri">Pack URI for UI Icon of the MainNavigationItem</param>
        /// <param name="displayMember">UI DisplayName for the MainNavigationItem</param>
        /// <param name="viewModelName">NavigationViewModel Name, which will be created/switched to when item is selected (clicked on)
        /// <param name="eventAggregator">Eventaggregator which will publish an Event when item is selected (clicked on)</param>
        public MainNavigationItemViewModel(string iconPackUri, string displayMember, string viewModelName, IEventAggregator eventAggregator)
        {
            IconPackUri = iconPackUri;
            DisplayMember = displayMember;
            ViewModelName = viewModelName;
            _eventAggregator = eventAggregator;

            CommandOpenNavigationViewModel = new DelegateCommand<string>(OnCommandOpenNavigationViewModel);
        }
        #endregion

        #region EventHandler
        /// <summary>
        /// Publish EventOpenNavigationOrDetailViewModel here for a NavigationViewModel, 
        /// therefore with EventArgs Id=0 und ViewModelName=navigationViewModelName
        /// </summary>
        /// <param name="navigationViewModelName">NavigationViewModel Name</param>
        /// <remarks>For a NavigationViewModel Id is always 0, as a NavigationViewModel for a specific Entity can only exists once.
        /// Therefore identification of a NavigationViewModel is always by <param name="navigationViewModelName"></param></remarks>
        private void OnCommandOpenNavigationViewModel(string navigationViewModelName)
        {
            _eventAggregator.GetEvent<EventOpenNavigationOrDetailViewModel>()
                .Publish(new EventOpenNavigationOrDetailViewModelArgs
                {
                    Id = 0,
                    ViewModelName = navigationViewModelName
                });
        }
        #endregion
    }
}
