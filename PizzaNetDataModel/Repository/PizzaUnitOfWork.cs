using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

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
        public OrderRepository Orders { get; set; }
        public SizeRepository Sizes { get; set; }
        public StateRepository States { get; set; }
        public UserRepository Users { get; set; }

        public bool RequestRollback { get; set; }

        public PizzaContext DbContext
        {
            get
            {
                return db;
            }
        }

        public PizzaUnitOfWork()
        {
            db = new PizzaContext();
            Ingredients = new IngredientRepository(db);
            Recipies = new RecipeRepository(db);
            Orders = new OrderRepository(db);
            Sizes = new SizeRepository(db);
            States = new StateRepository(db);
            Users = new UserRepository(db);
        }

        public PizzaUnitOfWork(bool reqRollback)
            : this()
        {
            RequestRollback = reqRollback;
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

        private object _syncRoot = new object();
        public T inTransaction<T>(Func<TransactionUnitOfWork, T> action)
        {
            lock (_syncRoot)
            {
                using (var tran = new TransactionScope())
                {
                    using (var tc = new TransactionUnitOfWork(this))
                    {
                        T result = action(tc);
                        if (!tc.RequestRollback)
                        {
                            tc.Db.Commit();
                            tran.Complete();
                        }
                        return result;
                    }
                }
            }
        }
        
        public void inTransaction(Action<TransactionUnitOfWork> action)
        {
            inTransaction( Uof => 
                {
                    action(Uof);
                    return 0;
                });
        }
    }
}
