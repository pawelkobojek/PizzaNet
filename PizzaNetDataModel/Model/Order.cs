using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetDataModel.Model
{
    public class Order
    {
        public int OrderID { get; set; }
        public DateTime Date { get; set; }
        public int CustomerPhone { get; set; }

        public int StateID { get; set; }
        public virtual State State { get; set; }
    }
}
