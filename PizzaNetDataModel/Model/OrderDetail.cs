using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetDataModel.Model
{
    /// <summary>
    /// Class representing particular pizza in an order.
    /// For example: Customer ordered small Margherita and medium Capriciosa - 
    /// there will be two istances of this class: one for Margherita and one
    /// for Capriciosa.
    /// </summary>
    public class OrderDetail : Entity
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int OrderDetailID { get; set; }

        /// <summary>
        /// Foreign key of related Order
        /// </summary>
        [ForeignKey("Order")]
        public int OrderID { get; set; }
        /// <summary>
        /// Related order.
        /// </summary>
        public virtual Order Order { get; set; }
        
        /// <summary>
        /// Foreign key of related size
        /// </summary>
        [ForeignKey("Size")]
        public int SizeID { get; set; }
        /// <summary>
        /// Size of the ordered pizza. It can small, medium or big.
        /// </summary>
        public virtual Size Size { get; set; }

        /// <summary>
        /// List of ingredients of the ordered pizza.
        /// </summary>
        public virtual ICollection<OrderIngredient> Ingredients { get; set; }
    }
}
