using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetDataModelEntityInfrastructure.Model;

namespace PizzaNetDataModel.Repository
{
    public interface IComplaintRepository : IRepository<Complaint, int>
    {
        Complaint Find(int id);
        List<Complaint> FindAll();
    }

    public class ComplaintRepository : IComplaintRepository
    {
        private readonly PizzaContext db;

        public ComplaintRepository(PizzaContext db)
        {
            this.db = db;
        }

        public Complaint Get(int id)
        {
            return db.Complaint.Find(id);
        }

        public void Insert(Complaint entity)
        {
            db.Complaint.Add(entity);
        }

        public void Update(Complaint entity, Complaint newEntity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Complaint entity)
        {
            throw new NotImplementedException();
        }

        public Complaint Find(int id)
        {
            throw new NotImplementedException();
        }

        public List<Complaint> FindAll()
        {
            return db.Complaint.ToList();
        }
    }
}
