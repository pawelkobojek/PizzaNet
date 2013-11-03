using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetDataModel.Repository
{
    /// <summary>
    /// Base class for every repository in a project.
    /// 
    /// </summary>
    /// <typeparam name="TEntity">Entity type for which the repository will be built</typeparam>
    /// <typeparam name="TKey">Type of key used to search for entities in a repository</typeparam>
    public interface IRepository<TEntity, in TKey> where TEntity : class
    {
        /// <summary>
        /// Gets the entity with a given key.
        /// </summary>
        /// <param name="id">Key of the entity being searched.</param>
        /// <returns>Searched entity.</returns>
        TEntity Get(TKey id);
        /// <summary>
        /// Inserts the entity to the database.
        /// </summary>
        /// <param name="entity">Entity to be insterted.</param>
        void Insert(TEntity entity);
        /// <summary>
        /// Updates the entity. 
        /// </summary>
        /// <param name="entity">Entity which to be updated.</param>
        /// <param name="newEntity">Entity representing new state</param>
        void Update(TEntity entity, TEntity newEntity);
        /// <summary>
        /// Removes the entity from the database.
        /// </summary>
        /// <param name="entity">Entity to be removed</param>
        void Delete(TEntity entity);
    }
}
