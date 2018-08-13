using Prism.Events;

namespace Soft.Ui.Shared.Events
{
    /// <summary>
    /// Event, after an Entity of a DetailViewModel was deleted.
    /// </summary>
    public class EventAfterDetailDeleted : PubSubEvent<EventAfterDetailDeletedArgs>
    {
    }

    /// <summary>
    /// Eventarguments, so that the ViewModel containing the Entity with the given Id can be identified (and removed) in the NavigationViewModel and in the MainViewModel.
    /// Id = Id of the (model) Entity, which is displayed in the View(model), 
    /// ViewModelName = Name of the responding DetailViewModel i.E. CustomerDetailViewModel
    /// </summary>
    /// <remarks>The combination of ViewModel and Entity-Id must be unique, otherwise it can not be identified in the NavigationViewModel and in the MainViewModel</remarks>
    public class EventAfterDetailDeletedArgs
    {
        public int Id { get; set; }
        public string ViewModelName { get; set; }
    }
}
