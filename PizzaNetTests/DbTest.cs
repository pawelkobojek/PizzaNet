using System;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PizzaNetCommon.Requests;
using PizzaNetDataModel;
using PizzaNetDataModel.Model;
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

        protected User admin;
        protected User emp;
        protected User customer;

        protected EmptyRequest adminRequest;
        protected EmptyRequest empRequest;
        protected EmptyRequest customerRequest;

        [TestInitialize]
        public void Initialize()
        {
            db = new PizzaUnitOfWork();
            admin = db.Users.Find("Admin");
            emp = db.Users.Find("Employee");
            customer = db.Users.Find("Customer");
            admin.Password = "123";
            emp.Password = "323";
            customer.Password = "1998";

            adminRequest = new EmptyRequest { Login = admin.Email, Password = admin.Password };
            empRequest = new EmptyRequest { Login = emp.Email, Password = emp.Password };
            customerRequest = new EmptyRequest { Login = customer.Email, Password = customer.Password };
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
