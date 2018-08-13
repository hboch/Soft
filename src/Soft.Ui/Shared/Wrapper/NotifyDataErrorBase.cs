using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Soft.Ui.Shared.Wrapper
{
    /// <summary>
    /// Abstract base class for EntityWrapper base class to manage property validation error messages. 
    /// Implements INotifyDataErrorInfo so that the Viewmodel is notified about property validation error messages. 
    /// Inherits from NotifyPropertyChangedBase.
    /// </summary>
    public abstract class NotifyDataErrorBase : NotifyPropertyChangedBase, INotifyDataErrorInfo
    {
        //Event, when the list of validation error messages changed
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        //Container for properties and their validation error messages
        private Dictionary<string, List<string>> _errorsByPropertyName = new Dictionary<string, List<string>>();

        /// <summary>
        /// Has at least one property a validation error message
        /// </summary>
        public bool HasErrors => _errorsByPropertyName.Any();

        /// <summary>
        /// Returns the list of validation error messages of a property.
        /// </summary>
        /// <param name="propertyName">Property Name</param>
        /// <returns>IEnumerable of all validation error messsages</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            IEnumerable returnValue = null;

            if (_errorsByPropertyName.ContainsKey(propertyName))
            {
                returnValue = _errorsByPropertyName[propertyName];
            }
            return returnValue;
        }

        /// <summary>
        /// Fire ErrorsChanged Event for given property and call OnPropertyChanged for property HasErrors.
        /// </summary>
        /// <param name="propertyName">Property name to fire event for</param>
        protected virtual void OnErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
            {
                ErrorsChanged.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
            OnPropertyChanged(nameof(HasErrors));
        }

        /// <summary>
        /// Add validation error message to property/validation error messages container for given property
        /// and call OnErrorsChanged for property
        /// </summary>
        /// <param name="propertyName">Property Name with validation error message</param>
        /// <param name="errorMessage">Validation Error Message</param>
        protected void AddError(string propertyName, string errorMessage)
        {
            //If property has no validation error messages yet then create one with the validation error message
            if (!_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName[propertyName] = new List<string> { errorMessage };
            }
            //else add validation error message to existing list
            else
            {
                _errorsByPropertyName[propertyName].Add(errorMessage);
            }

            //call OnErrorsChanged for property
            OnErrorsChanged(propertyName);
        }

        /// <summary>
        /// Remove property from property/validation error messages container 
        /// and call OnErrorsChanged for property
        /// </summary>
        /// <param name="propertyName">Property Name</param>
        protected void ClearErrors(string propertyName)
        {
            if (_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName.Remove(propertyName);

                //call OnErrorsChanged for property
                OnErrorsChanged(propertyName);
            }
        }
    }
}
