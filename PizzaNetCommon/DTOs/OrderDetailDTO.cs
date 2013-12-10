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
        public IList<OrderIngredientDTO> Ingredients { get; set; }

    }
}
