using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetDataModel.Model
{
    public class OrderIngredient
    {
        [Column(Order = 0), Key]
        public int IngredientID { get; set; }
        public virtual Ingredient Ingredient { get; set; }

        [Column(Order = 1), Key]
        public int OrderDetailID { get; set; }
        public virtual OrderDetail OrderDetail { get; set; }

        public int Quantity { get; set; }
    }
}
