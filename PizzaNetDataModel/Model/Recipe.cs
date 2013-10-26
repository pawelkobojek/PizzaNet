using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetDataModel.Model
{
    public class Recipe
    {
        public int RecipeID { get; set; }
        public String Name { get; set; }

        public virtual ICollection<Ingredient> Ingredients { get; set; }
    }
}
