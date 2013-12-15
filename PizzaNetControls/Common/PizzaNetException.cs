using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetControls.Common
{
    public class PizzaNetException : Exception
    {
        public PizzaNetException(string message) : base(message)
        {

        }
    }
}
