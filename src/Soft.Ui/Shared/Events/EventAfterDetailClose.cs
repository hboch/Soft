using Prism.Events;

namespace Soft.Ui.Shared.Events
{
    /// <summary>
    /// Event, after a DetailViewModel was closed.
    /// </summary>
    public class EventAfterDetailClose : PubSubEvent<EventAfterDetailCloseArgs>
    {
    }

    /// <summary>
    /// Eventarguments, so that the ViewModel containing the Entity with the given Id can be identified (and removed) in the MainViewModel.
    /// Id = Id of the (model) Entity, which is displayed in the View(model), 
    /// ViewModelName = Name of the responding DetailViewModel i.E. CustomerDetailViewModel
    /// </summary>
    /// <remarks>The combination of ViewModel and Entity-Id must be unique, otherwise it can not be identified in the MainViewModel</remarks>
    public class EventAfterDetailCloseArgs
    {
        public int Id { get; set; }
        public string ViewModelName { get; set; }
    }
}
