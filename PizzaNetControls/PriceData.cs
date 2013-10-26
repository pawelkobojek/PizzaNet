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

        public int PriceLow { get; set; }
        public int PriceMed { get; set; }
        public int PriceHigh { get; set; }

        public override string ToString()
        {
            return PriceLow.ToString() + " / " + PriceMed.ToString() + " / " + PriceHigh.ToString();
        }
    }
}
