using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetDataModel.Model
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        public int OrderID { get; set; }
        public DateTime Date { get; set; }
        public int CustomerPhone { get; set; }

        [ForeignKey("State")]
        public int StateID { get; set; }
        public virtual State State { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
