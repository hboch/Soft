using Soft.DataAccess;
using Soft.DataAccess.Shared.Repositories;
using Soft.Model;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Soft.Ui.Bc.BcBroker
{
    /// <summary>
    /// Repository für Broker Daten
    /// </summary>
    public class BrokerRepository : GenericRepository<Broker, SoftDbContext>, IBrokerRepository
    {
        /// <summary>
        /// Constructor to call base class constructor
        /// </summary>
        /// <param name="context"></param>
        public BrokerRepository(SoftDbContext context) 
            : base(context)
        {
        }

        /// <summary>
        /// Returns Broker of given Id from the Context including related Customer
        /// </summary>
        /// <param name="modelId">Id of the Broker to return</param>
        /// <returns></returns>
        public async override Task<Broker> FindByIdAsync(int modelId)
        {
            return await Context.Brokers
                .Include(kv => kv.Customers)
                .SingleAsync(kv => kv.Id == modelId);
        }
    }
}
