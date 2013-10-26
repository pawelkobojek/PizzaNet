using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetDataModel.Model
{
    [Table("Ingredients")]
    public class Ingredient
    {
        [Key]
        public int IngredientID { get; set; }
        public string Name { get; set; }
        public int StockQuantity { get; set; }
        public int NormalWeight { get; set; }
        public int ExtraWeight { get; set; }
        public decimal PricePerUnit { get; set; }

        public virtual ICollection<Recipe> Recipies { get; set; }
    }
}
