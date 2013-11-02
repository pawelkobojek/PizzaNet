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
    /// Class representing state of the order.
    /// </summary>
    public class State : Entity
    {
        [NotMapped]
        public const int NEW = 0;
        [NotMapped]
        public const int IN_REALISATION = 1;
        [NotMapped]
        public const int DONE = 2;

        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int StateID { get; set; }
        /// <summary>
        /// Value of the state. It maps as follows:
        /// New - 0
        /// In Realisation - 1
        /// Done - 2
        /// </summary>
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
