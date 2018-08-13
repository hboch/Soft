using Soft.Model;
using Soft.Ui.Shared.Wrapper;

namespace Soft.Ui.Bc.BcAccountManager
{
    /// <summary>
    /// Wraps all Viewmodel relevant properties of an Account Manager Entity
    /// </summary>
    public class AccountManagerWrapper : EntityWrapper<AccountManager>
    {
        /// <summary>
        /// Default constructor to call the base class constructor
        /// </summary>
        /// <param name="context"></param>
        public AccountManagerWrapper(AccountManager model) 
            : base(model)
        {
        }

        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public string Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}
