using Soft.Model.Shared;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Soft.DataAccess.Shared.Lookup
{
    /// <summary>
    /// Generic LookupDataService for a TEntity Type of a given DbContext
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class GenericLookupDataService<TEntity> : IGenericLookupDataService<TEntity>
         where TEntity : EntityBase
    {
        #region Fields
        private Func<DbContext> _dbContextCreator;
        #endregion

        #region Constructor
        /// <summary>
        /// Generate LookupDataService
        /// </summary>
        /// <param name="dbContextCreator">Delegate zur Erzeugung eines DbContext</param>
        public GenericLookupDataService(Func<DbContext> dbContextCreator)
        {
            _dbContextCreator = dbContextCreator;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Creates a list of LookupItems for all entities of type TEntity
        /// </summary>
        /// <returns>list of LookupItems for all entities of type TEntity</returns>
        public async Task<IEnumerable<LookupItem>> GetEntityLookupAsync()
        {
            //TODO Write unit test
            //Done: Write integration test
            using (var ctx = _dbContextCreator())
            {
                var entities = await ctx.Set<TEntity>().AsNoTracking().ToListAsync();

                List<LookupItem> lookupItems = new List<LookupItem>();

                foreach (var entity in entities)
                {
                    lookupItems.Add(new LookupItem()
                    {
                        Id = entity.Id,
                        DisplayMember = LookupDisplayMemberValue(entity)
                    });
                }
                return lookupItems;
            }
        }

        /// <summary>
        /// Returns the DisplayMember value of given entity
        /// </summary>
        /// <param name="entity">of which DisplayMember value has to be returned</param>
        /// <returns></returns>        
        /// <remark>Function has to be defined in the concrete implementation of LookupDatatService of TEntity</remark>
        public abstract string LookupDisplayMemberValue(TEntity entity);
        #endregion
    }
}
