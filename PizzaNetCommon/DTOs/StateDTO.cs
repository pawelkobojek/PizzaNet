using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PizzaNetCommon.DTOs
{
    public class StateDTO
    {
        public const int NEW = 0;
        public const int IN_REALISATION = 1;
        public const int DONE = 2;

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
