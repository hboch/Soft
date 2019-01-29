using CSharpFunctionalExtensions;
using Soft.Model.Shared;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace Soft.DataAccess.Shared.Repositories
{
    /// <summary>
    /// Generic Repository for a TEntity Type of a given DbContext
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    public class GenericRepository<TEntity, TDbContext> : IGenericRepository<TEntity>
        where TEntity : EntityBase
        where TDbContext : DbContext
    {
        #region Properties
        /// <summary>
        /// DbContext of Repository
        /// </summary>
        protected readonly TDbContext Context;
        #endregion

        #region Constructor 
        /// <summary>
        /// Constructor of Repository of a given DbContext
        /// </summary>
        /// <param name="dbContext">DbContext of Repository</param>
        protected GenericRepository(TDbContext dbContext)
        {
            this.Context = dbContext;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Adds an Entity to the still open DbContext
        /// </summary>
        /// <param name="entity"></param>
        public void Add(TEntity entity)
        {
            this.Context.Set<TEntity>().Add(entity);
        }

        /// <summary>
        /// Returns an Entity with given Id from the still open DbContext 
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns>Found Entity or Null, if not found.</returns>
        public virtual async Task<TEntity> FindByIdAsync(int entityId)
        {
            //TODO Write unit test
            //TODO Write integration test
            return await this.Context.Set<TEntity>().FindAsync(entityId);
        }
        /// <summary>
        /// Checks if Entity Framework ChangeTracker of the still open DbContext has detected changes
        /// </summary>
        /// <returns>True when changes, else False.</returns>
        public bool HasChanges()
        {
            return this.Context.ChangeTracker.HasChanges();
        }

        /// <summary>
        /// Removes Entity from the still open DbContext 
        /// </summary>
        /// <param name="entity"></param>
        public void Remove(TEntity entity)
        {
            this.Context.Set<TEntity>().Remove(entity);
        }

        /// <summary>
        /// Save all changes of the still open DbContext 
        /// </summary>
        /// <returns></returns>
        public async Task<Result> SaveAsync()
        {
            //TODO Write unit test
            //TODO Write integration test
            try
            {
                await this.Context.SaveChangesAsync();
                return Result.Ok();
            }
            catch (DbUpdateException ex)
            {
                string errorMessage = "";
                string exMessage = ex.GetFullMessage();
                if (exMessage.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                {
                    errorMessage = "Entity can not be deleted as it is referenced by another Entity. Reference has to be removed before." + exMessage;
                }
                return Result.Fail(errorMessage);
            }
        }

        //see https://msdn.microsoft.com/en-us/library/jj592904(v=vs.113).aspx
        //catch (DbUpdateConcurrencyException ex)
        //{
        //    var databaseValues = ex.Entries.Single().GetDatabaseValues();
        //    if (databaseValues == null)
        //    {
        //        await MessageDialogService.ShowInfoDialogAsync("The entity has been deleted by another user");
        //        RaiseDetailDeletedEvent(Id);
        //        return;
        //    }

        //    var result = await MessageDialogService.ShowOkCancelDialogAsync("The entity has been changed in "
        //     + "the meantime by someone else. Click OK to save your changes anyway, click Cancel "
        //     + "to reload the entity from the database.", "Question");

        //    if (result == MessageDialogResult.OK)
        //    {
        //        // Update the original values with database-values
        //        var entry = ex.Entries.Single();
        //        entry.OriginalValues.SetValues(entry.GetDatabaseValues());
        //        await saveFunc();
        //    }
        //    else
        //    {
        //        // Reload entity from database
        //        await ex.Entries.Single().ReloadAsync();
        //        await LoadAsync(Id);
        //    }
    }
    #endregion
}

