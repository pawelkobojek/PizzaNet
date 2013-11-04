using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetDataModel.Repository;

namespace PizzaNetTests
{
    public static class Extensions
    {
        public static void DetachAll(this ObjectContext objectContext)
        {

            foreach (var entry in objectContext.ObjectStateManager.GetObjectStateEntries(
                EntityState.Added | EntityState.Deleted | EntityState.Modified | EntityState.Unchanged))
            {
                if (entry.Entity != null)
                {
                    objectContext.Detach(entry.Entity);
                }
            }
        }

        public static ObjectContext ObjectContext(this PizzaUnitOfWork db)
        {
            return ((IObjectContextAdapter)db.DbContext).ObjectContext;
        }
    }
}
