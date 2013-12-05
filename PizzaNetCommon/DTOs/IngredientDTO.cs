using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetCommon.DTOs
{
    public class IngredientDTO
    {
        public int IngredientID { get; set; }

        public string Name { get; set; }

        public int StockQuantity { get; set; }

        public int NormalWeight { get; set; }

        public int ExtraWeight { get; set; }

        public decimal PricePerUnit { get; set; }

        public bool IsPartOfRecipe { get; set; }
    }
}
