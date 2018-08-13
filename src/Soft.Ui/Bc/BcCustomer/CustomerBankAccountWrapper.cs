using Soft.Model;
using Soft.Ui.Shared.Wrapper;

namespace Soft.Ui.Bc.BcCustomer
{
    /// <summary>
    /// Wraps all Viewmodel relevant properties of an customer Bank Account Entity
    /// </summary>
    public class CustomerBankAccountWrapper : EntityWrapper<BankAccount>
    {
        #region Constructor
        public CustomerBankAccountWrapper(BankAccount model) 
            : base(model)
        {
        }
        #endregion

        #region Properties
        public string AccountNo
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public bool IsForeignAccount
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        public string Notes
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        #endregion
    }
}

