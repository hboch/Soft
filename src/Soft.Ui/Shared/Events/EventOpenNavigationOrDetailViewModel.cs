using Prism.Events;

namespace Soft.Ui.Shared.Events
{
    /// <summary>
    /// Event, when a NavigationViewModel or a DetailViewModel for an Entity(-Id) should be created.
    /// </summary>
    public class EventOpenNavigationOrDetailViewModel : PubSubEvent<EventOpenNavigationOrDetailViewModelArgs>
    {
    }

    /// <summary>
    /// Eventarguments, so that a NavigationViewModel or a DetailViewModel containing the Entity(-Id) can be created.
    /// Id = 0 for a NavigationViewModel or = Id of the (model) Entity, which should be displayed in the View(model), 
    /// ViewModelName = Name of the responding NavigationViewModel or DetailViewModel i.E. CustomerDetailViewModel
    /// </summary>
    /// <remarks>The combination of ViewModel and Entity-Id must be unique, otherwise it can not be identified in the NavigationViewModel and in the MainViewModel</remarks>
    public class EventOpenNavigationOrDetailViewModelArgs
    {
        public int Id { get; set; }
        public string ViewModelName { get; set; }
    }
}
