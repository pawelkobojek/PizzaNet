using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetDataModel.Model
{
    [Table("Sizes")]
    public class Size : Entity
    {
        [Key]
        public int SizeID { get; set; }
        public double SizeValue { get; set; }
    }
}
