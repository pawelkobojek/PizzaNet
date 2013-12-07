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
    /// Interface for IngredientRepository
    /// </summary>
    public interface IIngredientRepository : IRepository<Ingredient, int>
    {
        /// <summary>
        /// Retrieves all Ingredients from the repository.
        /// </summary>
        /// <returns>List of all ingredients</returns>
        IList<Ingredient> FindAll();

        /// <summary>
        /// Retrieves all Ingredients from the repository with associated Recipies.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Ingredient> FindAllIncludeRecipies();

        /// <summary>
        /// Gets ingredient with a given name.
        /// If there is more than one ingredient with this name this method will take first.
        /// </summary>
        /// <param name="name">Name which will be searched.</param>
        /// <returns>First found in database ingredient object with a given name.</returns>
        Ingredient Get(string name);
    }

    /// <summary>
    /// Repository of Ingredients.
    /// It gives access to ingredients in a database.
    /// </summary>
    public class IngredientRepository : IIngredientRepository
    {
        private readonly PizzaContext db;

        /// <summary>
        /// Creates repository associated with given PizzaContext.
        /// </summary>
        /// <param name="ctx">PizzaContext which should be used for this repository</param>
        public IngredientRepository(PizzaContext ctx)
        {
            this.db = ctx;
        }

        public IList<Model.Ingredient> FindAll()
        {
            return db.Ingredients.ToList();
        }

        public IEnumerable<Model.Ingredient> Find(int id)
        {
            return db.Ingredients.Where(ing => ing.IngredientID == id);
        }

        public Model.Ingredient Get(int id)
        {
            return db.Ingredients.Find(id);
        }

        public void Update(Model.Ingredient entity, Ingredient newEntity)
        {
            var ing = db.Ingredients.Find(entity.IngredientID);
            ing.Name = newEntity.Name;
            ing.NormalWeight = newEntity.NormalWeight;
            ing.ExtraWeight = newEntity.ExtraWeight;
            ing.PricePerUnit = newEntity.PricePerUnit;
            //ing.Recipies = newEntity.Recipies;
            ing.StockQuantity = newEntity.StockQuantity;
            //db.Ingredients.Attach(ing);
        }

        public void Delete(Model.Ingredient entity)
        {
            db.Ingredients.Attach(entity);
            db.Ingredients.Remove(entity);
        }

        public void Insert(Ingredient entity)
        {
            db.Ingredients.Add(entity);
        }


        public Ingredient Get(string name)
        {
            return db.Ingredients.Where(ing => ing.Name == name).First();
        }


        public IEnumerable<Ingredient> FindAllIncludeRecipies()
        {
            return db.Ingredients.Include(ing => ing.Recipies).ToList();
        }

        public void DeleteAll()
        {
            foreach (var item in db.Ingredients)
            {
                db.Ingredients.Remove(item);
            }
        }
    }
}
