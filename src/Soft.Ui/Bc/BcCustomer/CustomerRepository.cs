using Soft.DataAccess;
using Soft.DataAccess.Shared.Repositories;
using Soft.Model;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Soft.Ui.Bc.BcCustomer
{
    /// <summary>
    /// Data repository for a Customer Entity
    /// </summary>
    public class CustomerRepository : GenericRepository<Customer, SoftDbContext>, ICustomerRepository
    {
        #region Constructor
        /// <summary>
        /// Constructor to call the base class constructor
        /// </summary>
        /// <param name="context">the DBContext</param>
        public CustomerRepository(SoftDbContext context)
            : base(context)
        {
        }
        #endregion

        #region Functions
        /// <summary>
        /// Returns (async) the Customer for the given Id from the still open DbContext.
        /// </summary>
        /// <param name="customerId">Id of the Customer Entity</param>
        /// <returns>Null or Customer Entity</returns>
        public override async Task<Customer> FindByIdAsync(int customerId)
        {
            //TODO Write unit test
            //Done Write integration test
            return await this.Context.Customers
                             .Include(k => k.BankAccounts)
                             .SingleOrDefaultAsync(customer => customer.Id == customerId);
        }

        /// <summary>
        /// Remove a customer bank account
        /// </summary>
        /// <param name="customerBankAccount">bank account to remove</param>
        public void RemoveBankAccount(BankAccount customerBankAccount)
        {
            this.Context.BankAccounts.Remove(customerBankAccount);
        }        
        #endregion
    }
}
