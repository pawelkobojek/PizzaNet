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
    /// Class representing many-to-many relation between ingredients and
    /// particural ordered pizzas.
    /// </summary>
    [Table("Order_Ingredients")]
    public class OrderIngredient : Entity
    {
        /// <summary>
        /// Foreign key of related ingredient
        /// </summary>
        [Column(Order = 0), Key, ForeignKey("Ingredient")]
        public int IngredientID { get; set; }
        /// <summary>
        /// Related ingredient
        /// </summary>
        public virtual Ingredient Ingredient { get; set; }

        /// <summary>
        /// Foreign key of related OrderDetail
        /// </summary>
        [Column(Order = 1), Key, ForeignKey("OrderDetail")]
        public int OrderDetailID { get; set; }
        /// <summary>
        /// Related OrderDetail
        /// </summary>
        public virtual OrderDetail OrderDetail { get; set; }

        /// <summary>
        /// Quantity of related ingredient in ordered pizza.
        /// </summary>
        public int Quantity { get; set; }
    }
}
