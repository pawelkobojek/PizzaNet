﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetDataModel.Model
{
    [Table("Recipies")]
    public class Recipe
    {
        [Key]
        public int RecipeID { get; set; }
        public String Name { get; set; }

        public virtual ICollection<Ingredient> Ingredients { get; set; }
    }
}