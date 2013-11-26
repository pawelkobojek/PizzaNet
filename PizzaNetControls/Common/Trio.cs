using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetControls.Common
{
    public class Trio<F, S, T> : Pair<F, S>
    {
        public T Third;
    }
}
