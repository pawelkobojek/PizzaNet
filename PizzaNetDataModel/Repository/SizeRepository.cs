using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetDataModel.Model;

namespace PizzaNetDataModel.Repository
{
    public interface ISizeRepository : IRepository<Size, int>
    {
        IEnumerable<Size> FindAll();
        IEnumerable<Size> Find(int id);
    }

    public class SizeRepository : ISizeRepository
    {
        private readonly PizzaContext db;

        public SizeRepository(PizzaContext ctx)
        {
            this.db = ctx;
        }

        public IEnumerable<Model.Size> FindAll()
        {
            return db.Sizes.ToList();
        }

        public IEnumerable<Model.Size> Find(int id)
        {
            return db.Sizes.Where(ing => ing.SizeID == id);
        }

        public Model.Size Get(int id)
        {
            return db.Sizes.Find(id);
        }

        public void Update(Model.Size entity, Size newEntity)
        {
            var size = db.Sizes.Find(entity.SizeID);
            size.SizeValue = newEntity.SizeValue;
        }

        public void Delete(Model.Size entity)
        {
            db.Sizes.Attach(entity);
            db.Sizes.Remove(entity);
        }

        public void Insert(Size entity)
        {
            db.Sizes.Add(entity);
        }
    }
}
