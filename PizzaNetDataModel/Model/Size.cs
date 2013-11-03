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
    public class Size : Entity
    {
        [NotMapped]
        public const double SMALL = 1;
        [NotMapped]
        public const double MEDIUM = 1.5;
        [NotMapped]
        public const double GREAT = 2;

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

        public override string ToString()
        {
            if (SizeValue == SMALL)
                return "Small";
            else if (SizeValue == MEDIUM)
                return "Medium";
            else if (SizeValue == GREAT)
                return "Great";
            return "";
        }
    }
}
