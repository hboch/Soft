namespace Soft.Model.Shared
{
    /// <summary>
    /// Lookup Item for an Entity. Allows to create a list of Lookup Items which can then be used i.E. in a ComboBox
    /// </summary>
    /// <remarks>An Item is composed of the unique Entity Id and a DisplayMember string which is displayed in the UI.</remarks>
    public class LookupItem
    {
        #region Properties
        public int Id { get; set; }
        public string DisplayMember { get; set; }
        #endregion

        #region Constructor
        public LookupItem()
        {
        }
        public LookupItem(EntityBase entity)
        {
            Id = entity.Id;
            DisplayMember = "";
        }
        #endregion
    }

    /// <summary>
    /// The Null Case: this NullLookupItem class should be used in place of C# null keyword.
    /// </summary>
    /// <remarks>see also NullObject Pattern https://en.wikipedia.org/wiki/Null_object_pattern </remarks>
    public class NullLookupItem : LookupItem
    {        
        public new int? Id { get { return null; } }
    }
}
