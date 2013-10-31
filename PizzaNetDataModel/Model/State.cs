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
        [NotMapped]
        public const int NEW = 0;
        [NotMapped]
        public const int IN_REALISATION = 1;
        [NotMapped]
        public const int DONE = 2;

        [Key]
        public int StateID { get; set; }
        public int StateValue { get; set; }

        public override string ToString()
        {
            switch (StateValue)
            {
                case NEW: return "New";
                case IN_REALISATION: return "In realization";
                case DONE: return "Done";
                default: return "Unknown";
            }
        }
    }
}
