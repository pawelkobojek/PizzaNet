using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetDataModel.Model
{
    public class OrderDetail
    {
        public int OrderDetailID { get; set; }

        public int OrderID { get; set; }
        public virtual Order Order { get; set; }

        public int SizeID { get; set; }
        public virtual Size Size { get; set; }
    }
}
