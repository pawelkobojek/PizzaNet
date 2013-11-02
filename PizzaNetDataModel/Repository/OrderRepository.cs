using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetDataModel.Model;

namespace PizzaNetDataModel.Repository
{
    public interface IOrderRepository : IRepository<Order, int>
    {
        IEnumerable<Order> FindAll();
        IEnumerable<Order> Find(int id);
    }

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
    }
}
