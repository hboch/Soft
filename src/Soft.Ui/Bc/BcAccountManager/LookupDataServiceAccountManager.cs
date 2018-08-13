using Soft.DataAccess.Shared.Lookup;
using Soft.DataAccess;
using Soft.Model;
using System;

namespace Soft.Ui.Bc.BcAccountManager
{
    public class LookupDataServiceAccountManager : GenericLookupDataService<AccountManager>, IGenericLookupDataService<AccountManager>
    {
        /// <summary>
        /// Default constructor to call the base class constructor
        /// </summary>
        /// <param name="dbContextCreator"></param>
        public LookupDataServiceAccountManager(Func<SoftDbContext> dbContextCreator)
            : base(dbContextCreator)
        {
        }
        public override string LookupDisplayMemberValue(AccountManager entity)
        {
            return entity.Name;
        }
    }
}
