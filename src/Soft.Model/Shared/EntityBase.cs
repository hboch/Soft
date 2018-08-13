namespace Soft.Model.Shared
{
    /// <summary>
    /// Abstract base class for an Entity.
    /// An Entity is characterized by a unique Id for all Enties of the same type.
    /// I.e. every Customer-Entity has a unique Id.
    /// </summary>
    /// <remarks>Id can be negative, when Entity is not saved yet</remarks>
    public abstract class EntityBase
    {
        #region Properties
        /// <summary>
        /// Unique Entity Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Display string to be used when Entity is shown in the UI for example to be selected in a ComboBox or NavigationControl
        /// </summary>
        public string LookupDisplayMember
        {
            get
            {
                return Id.ToString();
            }
        }
        #endregion
    }
}
