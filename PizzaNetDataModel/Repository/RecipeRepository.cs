using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetDataModel.Model;

namespace PizzaNetDataModel.Repository
{
    public interface IRecipeRepository : IRepository<Recipe, int>
    {
        IEnumerable<Recipe> FindAll();
        IEnumerable<Recipe> Find(int id);
    }

    public class RecipeRepository : IRecipeRepository
    {
        private readonly PizzaContext db;

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

        public Recipe Get(int id)
        {
            return db.Recipies.Find(id);
        }

        public void Update(Recipe entity)
        {
            db.Recipies.Attach(entity);
        }

        public void Delete(Recipe entity)
        {
            throw new NotImplementedException();
        }
    }
}
