using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetDataModel.Model
{
    [Table("Order_Details")]
    public class OrderDetail
    {
        [Key]
        public int OrderDetailID { get; set; }

        [ForeignKey("Order")]
        public int OrderID { get; set; }
        public virtual Order Order { get; set; }
        
        [ForeignKey("Size")]
        public int SizeID { get; set; }
        public virtual Size Size { get; set; }

        public virtual ICollection<OrderIngredient> Ingredients { get; set; }
    }
}
