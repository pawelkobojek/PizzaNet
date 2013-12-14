using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetCommon.DTOs
{
    public class RecipeDTO
    {
        public int RecipeID { get; set; }

        public string Name { get; set; }

        public List<OrderIngredientDTO> Ingredients { get; set; }
    }
}
