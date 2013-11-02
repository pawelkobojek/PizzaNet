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
    /// Class representing Ingredients.
    /// </summary>
    [Table("Ingredients")]
    public class Ingredient : Entity
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int IngredientID { get; set; }
        /// <summary>
        /// String value representing name of the ingredient.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Current quantity in stock.
        /// </summary>
        public int StockQuantity { get; set; }
        /// <summary>
        /// Normal, default amount of given ingredient in a recipe.
        /// </summary>
        public int NormalWeight { get; set; }
        /// <summary>
        /// Extra amount of given ingredient in a recipe.
        /// </summary>
        public int ExtraWeight { get; set; }
        /// <summary>
        /// Price of the ingredient (per unit).
        /// </summary>
        public decimal PricePerUnit { get; set; }

        /// <summary>
        /// Navigation property.
        /// </summary>
        public virtual ICollection<Recipe> Recipies { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
