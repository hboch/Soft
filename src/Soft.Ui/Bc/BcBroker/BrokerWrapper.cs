using Soft.Model;
using Soft.Ui.Shared.Wrapper;
using System.Collections.Generic;

namespace Soft.Ui.Bc.BcBroker
{
    /// <summary>
    /// Wraps all Viewmodel relevant properties of a Broker Entity
    /// </summary>
    public class BrokerWrapper : EntityWrapper<Broker>
    {
        public BrokerWrapper(Broker model) 
            : base(model)
        {
        }

        public int Id { get { return Entity.Id; } }

        /// <summary>
        /// A Broker must have a CustomerNo
        /// </summary>
        public string CustomerNo
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        #region Validierung
        /// <summary>
        /// Validates a property, when it is called in a property setter. Add a validation for each propertyName here.
        /// </summary>
        /// <param name="propertyName"></param>
        protected override IEnumerable<string> ValidatePorperty(string propertyName)
        {
            //Property Validation
            switch (propertyName)
            {
                //Validate property Name
                case nameof(Name):
                    if (Entity.Name.Length < 2) yield return "Name must be at least 2 characters.";
                    if (!Entity.Name.Contains(" ")) yield return "Name must contain a space character.";                    
                    break;
            }
        }
        #endregion
    }
}
