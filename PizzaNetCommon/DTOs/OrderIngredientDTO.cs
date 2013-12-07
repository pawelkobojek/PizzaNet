using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetCommon.DTOs
{
    public class OrderIngredientDTO
    {
        public double Quantity { get; set; }
        public int IngredientID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
