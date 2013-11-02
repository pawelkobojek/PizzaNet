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
    /// Class representing size of the pizza.
    /// </summary>
    [Table("Sizes")]
    public class Size : Entity
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int SizeID { get; set; }

        /// <summary>
        /// Value of the size. It maps as follows:
        /// Small - 1.0
        /// Medium - 1.5
        /// Big - 2.0
        /// </summary>
        public double SizeValue { get; set; }
    }
}
