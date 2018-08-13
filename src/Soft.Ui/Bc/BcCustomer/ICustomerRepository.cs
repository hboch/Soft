using Soft.DataAccess.Shared.Repositories;
using Soft.Model;

namespace Soft.Ui.Bc.BcCustomer
{
    /// <summary>
    /// Interface of a data repository for a Customer Entity
    /// </summary>
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        /// <summary>
        /// Remove a Bank Account
        /// </summary>
        /// <param name="bankAccount"> to remove</param>
        void RemoveBankAccount(BankAccount bankAccount);
    }
}