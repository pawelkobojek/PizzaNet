using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetDataModel.Model;

namespace PizzaNetDataModel.Repository
{
    public interface IStateRepository : IRepository<State, int>
    {
        List<State> FindAll();

        State Find(int stateValue);
    }

    public class StateRepository : IStateRepository
    {
        private readonly PizzaContext db;

        public StateRepository(PizzaContext ctx)
        {
            db = ctx;
        }

        public List<State> FindAll()
        {
            return db.States.ToList();
        }

        public State Get(int id)
        {
            return db.States.Find(id);
        }

        public void Insert(State entity)
        {
            db.States.Add(entity);
        }

        public void Update(State entity, State newEntity)
        {
            var state = db.States.Find(entity.StateID);
            state.StateValue = newEntity.StateValue;
        }

        public void Delete(State entity)
        {
            db.States.Remove(entity);
        }


        public State Find(int stateValue)
        {
            return db.States.Where(s => s.StateValue == stateValue).First();
        }
    }
}
