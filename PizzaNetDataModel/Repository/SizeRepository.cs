using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetDataModel.Model;

namespace PizzaNetDataModel.Repository
{
    /// <summary>
    /// Interface for SizeRepository.
    /// </summary>
    public interface ISizeRepository : IRepository<Size, int>
    {
        /// <summary>
        /// Retrieves all Sizes from the repository.
        /// </summary>
        /// <returns>List of all sizes.</returns>
        IEnumerable<Size> FindAll();
    }

    /// <summary>
    /// Repository of Sizes.
    /// It gives access to sizes hold in a database.
    /// </summary>
    public class SizeRepository : ISizeRepository
    {
        private readonly PizzaContext db;

        /// <summary>
        /// Creates repository associated with given PizzaContext.
        /// </summary>
        /// <param name="ctx">PizzaContext which should be used for this repository</param>
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
