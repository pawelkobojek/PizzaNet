using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetDataModel.Model;

namespace PizzaNetDataModel.Repository
{
    public interface IUserRepository : IRepository<User, int>
    {
        User Find(string login);
        IList<User> FindAll();
    }

    public class UserRepository : IUserRepository
    {
        private readonly PizzaContext db;

        public UserRepository(PizzaContext ctx)
        {
            db = ctx;
        }

        public IList<User> FindAll()
        {
            return db.Users.ToList();
        }

        public User Get(int id)
        {
            return db.Users.Find(id);
        }

        public void Insert(User entity)
        {
            db.Users.Add(entity);
        }

        public void Update(User entity, User newEntity)
        {
            var user = db.Users.Find(entity.UserID);
            user.Rights = newEntity.Rights;
            user.Phone = newEntity.Phone;
            user.Name = newEntity.Name;
            user.Email = newEntity.Email;
            user.Address = newEntity.Address;
        }

        public void Delete(User entity)
        {
            db.Users.Remove(entity);
        }

        public User Find(string p)
        {
            return db.Users.SingleOrDefault(u => u.Email == p);
        }
    }
}
