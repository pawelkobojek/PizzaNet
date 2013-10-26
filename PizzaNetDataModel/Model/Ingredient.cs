using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetDataModel.Model
{
    public class Ingredient
    {
        public int IngredientID { get; set; }
        public String Name { get; set; }
        public int StockQuantity { get; set; }
        public int NormalWeight { get; set; }
        public int ExtraWeight { get; set; }
        public int PricePerUnit { get; set; }

        public virtual ICollection<Recipe> Recipies { get; set; }
    }
}
