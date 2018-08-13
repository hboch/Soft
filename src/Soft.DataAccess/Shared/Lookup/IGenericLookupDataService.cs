 using Soft.Model.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Soft.DataAccess.Shared.Lookup
{
    /// <summary>
    ///  Generic LookupDataService for a TEntity Type of a given DbContext
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IGenericLookupDataService<TEntity>
        where TEntity:EntityBase
    {
        /// <summary>
        /// Creates a list of LookupItems for all entities of type TEntity
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<LookupItem>> GetEntityLookupAsync();

        /// <summary>
        /// Returns the DisplayMember value of given entity
        /// </summary>
        /// <param name="entity">of which DisplayMember value has to be returned</param>
        /// <returns></returns>        
        /// <remark>Function has to be defined in the concrete implementation of LookupDatatService of TEntity</remark>
        string LookupDisplayMemberValue(TEntity entity);
    }
}