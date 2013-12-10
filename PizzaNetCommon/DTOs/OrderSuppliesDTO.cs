using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetCommon.DTOs
{
    public class OrderSuppliesDTO
    {
        public int IngredientID { get; set; }
        public string Name { get; set; }
        public int OrderValue { get; set; }
        public int StockQuantity { get; set; }
    }
}
