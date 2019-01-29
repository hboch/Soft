using CSharpFunctionalExtensions;
using Soft.Model.Shared;
using System.Threading.Tasks;

namespace Soft.DataAccess.Shared.Repositories
{
    /// <summary>
    /// Interface of generic Repository for a TEntity Type of a given DbContext
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IGenericRepository<TEntity>
        where TEntity : EntityBase
    {
        /// <summary>
        /// Find Entity by its Id
        /// </summary>
        /// <param name="entityId">Id of Entity</param>
        /// <returns></returns>
        Task<TEntity> FindByIdAsync(int entityId);

        /// <summary>
        /// Save Repository
        /// </summary>
        /// <returns></returns>
        Task<Result> SaveAsync();

        /// <summary>
        /// Checks whether Repository has Changes
        /// </summary>
        /// <returns></returns>
        bool HasChanges();

        /// <summary>
        /// Adds Entity to Repository 
        /// </summary>
        /// <param name="model"></param>
        void Add(TEntity model);

        /// <summary>
        /// Removes Entity from Repository
        /// </summary>
        /// <param name="model"></param>
        void Remove(TEntity model);
    }
}
