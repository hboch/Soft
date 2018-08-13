using Soft.DataAccess;
using Soft.DataAccess.Shared.Lookup;
using Soft.Model;
using System;

namespace Soft.Ui.Bc.BcCustomer
{
    public class LookupDataServiceCustomer : GenericLookupDataService<Customer>, IGenericLookupDataService<Customer>
    {
        public LookupDataServiceCustomer(Func<SoftDbContext> dbContextCreator)
            :base(dbContextCreator)
        {                
        }
        public override string LookupDisplayMemberValue(Customer entity)
        {
            return entity.CustomerNo;
        }
    }
}
