using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebClient.Models
{
    public class OrderInfo
    {
        public int[] Ingredients { get; set; }
        public string[] Quantities { get; set; }
        public string Size { get; set; }
    }
}