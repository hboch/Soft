using Soft.Model.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Soft.Ui.Shared.Wrapper
{
    /// <summary>
    /// Generic class to wrap an Entity
    /// </summary>
    /// <typeparam name="TEntity">EntityBase</typeparam>
    public abstract class EntityWrapper<TEntity> : NotifyDataErrorBase
         where TEntity : EntityBase
    {
        #region Properties
        public TEntity Entity { get; }
        #endregion

        #region Constructor
        /// <summary>
        /// Construct a Wrapper for an Entity
        /// </summary>
        /// <param name="entity"></param>
        public EntityWrapper(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            Entity = entity;
        }
        #endregion

        #region PropertySetterGetter
        /// <summary>
        /// Read property value using reflection
        /// </summary>
        /// <typeparam name="TValue">Property Value Type</typeparam>
        /// <param name="propertyName">Property Name</param>
        /// <returns>Value of Property</returns>
        protected virtual TValue GetValue<TValue>([CallerMemberName] string propertyName = null)
        {
            return (TValue)typeof(TEntity).GetProperty(propertyName).GetValue(Entity);
        }

        /// <summary>
        /// Set property value, if new value is different from old value using reflection
        /// </summary>
        /// <typeparam name="TValue">Der Typ des Property Values</typeparam>
        /// <typeparam name="TValue">Property Value Type</typeparam>
        /// <param name="propertyName">Property Name</param>
        protected virtual void SetValue<TValue>(TValue value, [CallerMemberName] string propertyName = null)
        {
            var propertyInfo = Entity.GetType().GetProperty(propertyName);
            var currentValue = propertyInfo.GetValue(Entity);
            if (!Equals(currentValue, value))
            {
                propertyInfo.SetValue(Entity, value);
                OnPropertyChanged(propertyName);
                ValidatePropertyInternal(propertyName, value);
            }
        }
        #endregion

        /// <summary>
        /// Validates Data Annotations of the property and property validatons defined in the concrete Wrapper. 
        /// </summary>
        /// <param name="propertyName">Property Name</param>
        /// <param name="currentValue">Current Property Value</param>
        private void ValidatePropertyInternal(string propertyName, object currentValue)
        {
            ClearErrors(propertyName);
            //1. Validate Data Annotations
            ValidateDataAnnotations(propertyName, currentValue);

            //2. Validate validatons defined in the concrete Wrapper class
            ValidateCustomErrors(propertyName);
        }
        /// <summary>
        /// Validate Data Annotations of the property
        /// </summary>
        /// <param name="propertyName">Property Name</param>
        /// <param name="currentValue">Current Property Value</param>
        private void ValidateDataAnnotations(string propertyName, object currentValue)
        {
            var validationContext = new ValidationContext(Entity) { MemberName = propertyName };
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateProperty(currentValue, validationContext, validationResults);
            foreach (var validationResult in validationResults)
            {
                AddError(propertyName, validationResult.ErrorMessage);
            }
        }

        /// <summary>
        /// Validate validatons defined in the concrete Wrapper class
        /// </summary>
        /// <param name="propertyName">Property Name</param>
        private void ValidateCustomErrors(string propertyName)
        {
            var errorMessages = ValidatePorperty(propertyName);
            if (errorMessages != null)
            {
                foreach (var errorMessage in errorMessages)
                {
                    AddError(propertyName, errorMessage);
                }
            }
        }

        /// <summary>
        /// Function to be overwritten in the concrete Wrapper class to validate validatons defined in the concrete Wrapper class
        /// </summary>
        /// <param name="propertyName">Der Name des zu validierenden Properties</param>
        /// <returns></returns>
        protected virtual IEnumerable<string> ValidatePorperty(string propertyName)
        {
            return null;
        }
    }
}
