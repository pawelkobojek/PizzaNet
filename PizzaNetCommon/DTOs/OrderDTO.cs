using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetCommon.DTOs
{
    public class OrderDTO
    {
        public int OrderID { get; set; }
        public DateTime Date { get; set; }
        public int CustomerPhone { get; set; }
        public string Address { get; set; }
//        public int StateID{ get; set; }
        public virtual StateDTO State { get; set; }
        public IList<OrderDetailDTO> OrderDetailsDTO { get; set; }
    }
}
