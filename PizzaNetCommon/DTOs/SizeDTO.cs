using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PizzaNetCommon.DTOs
{
    public class SizeDTO
    {
        public const double SMALL = 1;
        public const double MEDIUM = 1.5;
        public const double GREAT = 2;

        public int SizeID { get; set; }
        public double SizeValue { get; set; }

        public override string ToString()
        {
            if (SizeValue == SMALL)
                return "Small";
            else if (SizeValue == MEDIUM)
                return "Medium";
            else if (SizeValue == GREAT)
                return "Great";
            return "";
        }
    }
}
