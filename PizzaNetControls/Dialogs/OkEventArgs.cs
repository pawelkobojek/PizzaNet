
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetControls.Dialogs
{
    public class OkEventArgs : EventArgs
    {
        public string Result { get; set; }
        public bool IsValid { get; set; }
    }
}
