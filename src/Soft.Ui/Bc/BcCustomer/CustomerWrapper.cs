using Soft.Model;
using Soft.Ui.Shared.Wrapper;
using System;
using System.Collections.Generic;

namespace Soft.Ui.Bc.BcCustomer
{
    /// <summary>
    /// Wraps all Viewmodel relevant properties of a Customer Entity
    /// </summary>
    public class CustomerWrapper : EntityWrapper<Customer>
    {
        #region Properties

        public int Id { get { return Entity.Id; } }

        public string Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string CustomerNo
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public DateTime CustomerSince
        {
            get
            { return GetValue<DateTime>(); }
            set { SetValue(value); }
        }
        public int? AccountManagerId
        {
            get { return GetValue<int?>(); }
            set { SetValue(value); }
        }
        public string InternetAdress
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        //public AccountManagerWrapper AccountManager { get; private set; }
        //public ObservableCollection<Account> Accounts { get; private set; }
        #endregion

        #region Constructor
        public CustomerWrapper(Customer model)
            : base(model)
        {
            //InitializeComplexProperties(model);
            //InitializeCollectionProperties(model);
        }

        //private void InitializeCollectionProperties(Customer model)
        //{
        //    if (model.Accounts == null)
        //    {
        //        throw new ArgumentException("Bank Account must not be Null.");
        //    }
        //    Accounts = new ObservableCollection<Account>();
        //    foreach (var bankAccount in Model.Accounts)
        //    {
        //        Accounts.Add(bankAccount);
        //    };
        //}

        //private void InitializeComplexProperties(Customer model)
        //{
        //    if (model.AccountManager == null)
        //    {
        //        throw new ArgumentException("Account Manager must not be Null.");
        //    }
        //    AccountManager = new AccountManagerWrapper(model.AccountManager);
        //}
        #endregion

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

                //Validate property CustomerSince
                case nameof(CustomerSince):
                    if (Entity.CustomerSince > DateTime.Now) yield return "Customer Since must not be in the future.";
                    if (Entity.CustomerSince > DateTime.MaxValue) yield return "Customer Since must not be after " + DateTime.MaxValue.ToString();
                    if (Entity.CustomerSince < DateTime.MinValue) yield return "Customer Since must not be before " + DateTime.MinValue.ToString();

                    break;
            }
        }
        #endregion
    }
}
