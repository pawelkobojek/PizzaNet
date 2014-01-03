using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PizzaNetCommon.DTOs
{
    public class OrderInfoDTO
    {
        public int[] Ingredients { get; set; }
        public string[] Quantities { get; set; }
        public string Size { get; set; }
    }
}