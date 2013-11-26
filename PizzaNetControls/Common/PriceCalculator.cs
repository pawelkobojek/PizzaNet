using PizzaNetDataModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetControls.Common
{
    public static class PriceCalculator
    {
        public static PriceData GetPrices(Recipe r, Size[] sizes)
        {
            PriceData result = new PriceData();
            foreach (var i in r.Ingredients)
            {
                if (sizes.Count()!=3) throw new Exception("Invalid sizes collection");
                result.PriceLow += i.NormalWeight*(double)i.PricePerUnit * sizes[0].SizeValue;
                result.PriceMed += i.NormalWeight*(double)i.PricePerUnit * sizes[1].SizeValue;
                result.PriceHigh += i.NormalWeight*(double)i.PricePerUnit * sizes[2].SizeValue;
            }
            return result;
        }

        public static double Calculate(IEnumerable<IngredientsRow> ingredients, double sizeValue)
        {
            double result = 0;
            foreach (var i in ingredients)
            {
                result += (double)i.CurrentQuantity * (double)i.Ingredient.PricePerUnit * sizeValue;
            }
            return result;
        }
    }
}
