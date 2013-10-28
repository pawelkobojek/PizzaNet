using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetDataModel.Model;

namespace PizzaNetDataModel.Repository
{
    public interface IIngredientRepository : IRepository<Ingredient, int>
    {
        IEnumerable<Ingredient> FindAll();
        IEnumerable<Ingredient> Find(int id);
    }

    public class IngredientRepository : IIngredientRepository
    {
        private readonly PizzaContext db;

        public IngredientRepository(PizzaContext ctx)
        {
            this.db = ctx;
        }

        public IEnumerable<Model.Ingredient> FindAll()
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

        public void Update(Model.Ingredient entity)
        {
            db.Ingredients.Add(entity);
        }

        public void Delete(Model.Ingredient entity)
        {
            db.Ingredients.Remove(entity);
        }
    }
}
