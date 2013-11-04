using System;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PizzaNetDataModel;
using PizzaNetDataModel.Repository;


namespace PizzaNetTests
{
    /// <summary>
    /// Base class for tests involving database access.
    /// It keeps the connection object and provides with method allowing auto-rollbacks of transactions.
    /// </summary>
    [TestClass]
    public abstract class DbTest
    {
        protected PizzaUnitOfWork db;

        [TestInitialize]
        public void Initialize()
        {
            db = new PizzaUnitOfWork();
        }

        [TestCleanup]
        public void Cleanup()
        {
            db.Dispose();
        }

        protected void InAutoRollbackTransaction(Action<TransactionUnitOfWork> action)
        {
            db.inTransaction(uof =>
                {
                    uof.RequestRollback = true;
                    action(uof);
                });
        }
    }
}
