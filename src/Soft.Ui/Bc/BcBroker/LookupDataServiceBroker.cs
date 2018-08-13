using Soft.DataAccess;
using Soft.DataAccess.Shared.Lookup;
using Soft.Model;
using System;

namespace Soft.Ui.Bc.BcBroker
{
    public class LookupDataServiceBroker : GenericLookupDataService<Broker>, IGenericLookupDataService<Broker>
    {
        public LookupDataServiceBroker(Func<SoftDbContext> dbContextCreator)
            : base(dbContextCreator)
        {
        }
        public override string LookupDisplayMemberValue(Broker entity)
        {
            return entity.Name;
        }
    }
}
