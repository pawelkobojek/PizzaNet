using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetControls.Validation
{
    public class ValidationEventArgs : EventArgs
    {
        public bool Result { get; set; }
    }
}
