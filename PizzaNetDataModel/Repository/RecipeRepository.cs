using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using PizzaNetDataModel.Model;

namespace PizzaNetDataModel.Repository
{
    /// <summary>
    /// Interface for RecipeRepository.
    /// </summary>
    public interface IRecipeRepository : IRepository<Recipe, int>
    {
        /// <summary>
        /// Retrieves all Recipies in database.
        /// </summary>
        /// <returns>List of all recipies in database.</returns>
        IEnumerable<Recipe> FindAll();

        IEnumerable<Recipe> FindAllEagerly();

        IEnumerable<Recipe> FindEagerly(int id);
    }

    /// <summary>
    /// Repository of Recipies.
    /// It gives access to the recipies hold in a database.
    /// </summary>
    public class RecipeRepository : IRecipeRepository
    {
        private readonly PizzaContext db;

        /// <summary>
        /// Creates repository associated with given PizzaContext.
        /// </summary>
        /// <param name="ctx">PizzaContext which should be used for this repository</param>
        public RecipeRepository(PizzaContext ctx)
        {
            db = ctx;
        }

        public IEnumerable<Recipe> FindAll()
        {
            return db.Recipies.ToList();
        }

        public IEnumerable<Recipe> Find(int id)
        {
            return db.Recipies.Where(r => r.RecipeID == id);
        }

        public IEnumerable<Recipe> FindEagerly(int id)
        {
            return db.Recipies.Include(r => r.Ingredients).Where(r => r.RecipeID == id);
        }

        public Recipe Get(int id)
        {
            return db.Recipies.Find(id);
        }

        public void Update(Recipe entity, Recipe newEntity)
        {
            db.Recipies.Attach(entity);
        }

        public void Update(Recipe entity)
        {
            db.Recipies.Attach(entity);
        }

        public void Delete(Recipe entity)
        {
            db.Recipies.Remove(entity);
        }


        public void Insert(Recipe entity)
        {
            db.Recipies.Add(entity);
        }


        public IEnumerable<Recipe> FindAllEagerly()
        {
            return db.Recipies.Include(r => r.Ingredients).ToList();
        }
    }
}
