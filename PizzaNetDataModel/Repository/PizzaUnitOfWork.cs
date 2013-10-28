using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetDataModel.Repository
{
    public interface IPizzaUnitOfWork
    {
        void Commit();
    }

    public class PizzaUnitOfWork : IPizzaUnitOfWork, IDisposable
    {
        private PizzaContext db;

        public IngredientRepository Ingredients { get; set; }
        public RecipeRepository Recipies { get; set; }


        public PizzaUnitOfWork()
        {
            db = new PizzaContext();
            Ingredients = new IngredientRepository(db);
            Recipies = new RecipeRepository(db);
        }

        public void Commit()
        {
            db.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
        }
    }
}
