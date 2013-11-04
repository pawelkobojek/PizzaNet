using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetDataModel.Repository
{
    public class TransactionUnitOfWork : IDisposable
    {
        public PizzaUnitOfWork Db { get; set; }
        public bool RequestRollback { get; set; }

        public TransactionUnitOfWork(PizzaUnitOfWork db)
        {
            Db = db;
        }

        public void Dispose()
        {
            
        }
    }
}
