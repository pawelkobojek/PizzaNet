using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PizzaNetControls
{
    public class PriceData
    {
        private static PriceData _empty = new PriceData() { PriceLow=0, PriceMed=0, PriceHigh=0};
        public static PriceData Empty
        {
            get { return _empty; }
        }

        public double PriceLow { get; set; }
        public double PriceMed { get; set; }
        public double PriceHigh { get; set; }

        public override string ToString()
        {
            return String.Format("{0:0.00} / {1:0.00} / {2:0.00}", PriceLow, PriceMed, PriceHigh);
        }
    }
}
