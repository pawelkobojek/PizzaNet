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
    /// Interface for OrderRepository.
    /// </summary>
    public interface IOrderRepository : IRepository<Order, int>
    {
        /// <summary>
        /// Retrieves all Orders in the database.
        /// It uses lazy loading. In order to get eagerly loaded data use FindAllEagearly() method.
        /// </summary>
        /// <returns>List of all orders.</returns>
        IEnumerable<Order> FindAll();
        /// <summary>
        /// Retrieves all Orders in the database in a eager manner.
        /// In order to use lazy loading call FindAll() method.
        /// </summary>
        /// <returns>List of all orders.</returns>
        IEnumerable<Order> FindAllEagerly();

        /// <summary>
        /// Retrieves all Orders in the database satisfying a condition 'predicate' in a eager manner.
        /// In order to use lazy loading call FindAll() method.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<Order> FindAllEagerlyWhere(Func<Order, bool> predicate);
    }
    
    /// <summary>
    /// Repository of Orders.
    /// It gives access to orders in database.
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private PizzaContext db;

        public OrderRepository(PizzaContext ctx)
        {
            db = ctx;
        }

        public IEnumerable<Order> FindAll()
        {
            return db.Orders.ToList();
        }

        public IEnumerable<Order> Find(int id)
        {
            return db.Orders.Where(o => o.OrderID == id);
        }

        public Order Get(int id)
        {
            return db.Orders.Where(o => o.OrderID == id).First();
        }

        public void Insert(Order entity)
        {
            db.Orders.Add(entity);
        }

        public void Update(Order entity, Order newEntity)
        {
            var order = db.Orders.Find(entity.OrderID);
            order.Address = newEntity.Address;
            order.CustomerPhone = newEntity.CustomerPhone;
            order.Date = newEntity.Date;
            order.OrderDetails = newEntity.OrderDetails;
            order.State = newEntity.State;
            order.StateID = newEntity.StateID;
        }

        public void Delete(Order entity)
        {
            db.Orders.Remove(entity);
        }


        public IEnumerable<Order> FindAllEagerly()
        {
            return db.Orders
                .Include(o => o.OrderDetails.Select(od => od.Ingredients))
                .Include(o => o.OrderDetails.Select(od => od.Ingredients.Select(ing => ing.Ingredient)))
                .Include(o=>o.OrderDetails.Select(od=>od.Size))
                .Include(o => o.State)
                .ToList();
        }

        public IEnumerable<Order> FindAllEagerlyWhere(Func<Order, bool> predicate)
        {
            return db.Orders
                .Include(o => o.OrderDetails.Select(od => od.Ingredients))
                .Include(o => o.OrderDetails.Select(od => od.Ingredients.Select(ing => ing.Ingredient)))
                .Include(o => o.OrderDetails.Select(od => od.Size))
                .Include(o => o.State).Where(predicate)
                .ToList();
        }
    }
}
