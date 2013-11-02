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
    /// Class representing Recipes.
    /// </summary>
    [Table("Recipies")]
    public class Recipe : Entity
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        [Key]
        public int RecipeID { get; set; }

        /// <summary>
        /// String value representing name of the recipe.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// List of ingredients the recipe consists of.
        /// </summary>
        public virtual ICollection<Ingredient> Ingredients { get; set; }
    }
}