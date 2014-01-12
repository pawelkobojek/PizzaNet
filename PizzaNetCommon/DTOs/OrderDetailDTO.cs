using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PizzaNetCommon.DTOs
{
    public class OrderDetailDTO
    {
        public int OrderDetailID { get; set; }
        public SizeDTO Size { get; set; }
        public List<OrderIngredientDTO> Ingredients { get; set; }
        public decimal Price
        {
            get
            {
                decimal priceVal = 0;
                foreach (var item in Ingredients)
                {
                    priceVal += item.Price * (decimal)item.Quantity;
                }
                return priceVal * (decimal)Size.SizeValue;
            }
        }
    }
}
