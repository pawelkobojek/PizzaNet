using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetDataModel.Model
{
    [Table("States")]
    public class State
    {
        [Key]
        public int StateID { get; set; }
        public int StateValue { get; set; }
    }
}
