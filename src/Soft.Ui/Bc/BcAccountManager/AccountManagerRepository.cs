using Soft.DataAccess;
using Soft.DataAccess.Shared.Repositories;
using Soft.Model;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Soft.Ui.Bc.BcAccountManager
{
    public class AccountManagerRepository : GenericRepository<AccountManager, SoftDbContext>, IAccountManagerRepository
    {
        /// <summary>
        /// Default constructor to call the base class constructor
        /// </summary>
        /// <param name="context"></param>
        public AccountManagerRepository(SoftDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Reads the Account Manager for the given ID from the database
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public async override Task<AccountManager> FindByIdAsync(int modelId)
        {
            return await Context.AccountManagers
                .SingleAsync(kv => kv.Id == modelId);
        }
    }
}
