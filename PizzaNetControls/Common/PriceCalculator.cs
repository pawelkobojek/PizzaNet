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
        private const double BASE_PRICE = 3.0;
        public static PriceData GetPrices(Recipe r, Size[] sizes)
        {
            PriceData result = new PriceData() { PriceLow =  BASE_PRICE * sizes[0].SizeValue,
                PriceMed = BASE_PRICE * sizes[1].SizeValue, PriceHigh = BASE_PRICE * sizes[2].SizeValue };
            foreach (var i in r.Ingredients)
            {
                if (sizes.Count() != 3) throw new Exception("Invalid sizes collection");
                result.PriceLow += i.NormalWeight * (double)i.PricePerUnit * sizes[0].SizeValue;
                result.PriceMed += i.NormalWeight * (double)i.PricePerUnit * sizes[1].SizeValue;
                result.PriceHigh += i.NormalWeight * (double)i.PricePerUnit * sizes[2].SizeValue;
            }
            return result;
        }

        public static double Calculate(IEnumerable<IngredientsRow> ingredients, double sizeValue)
        {
            double result = 3 * sizeValue;
            foreach (var i in ingredients)
            {
                result += (double)i.CurrentQuantity * (double)i.Ingredient.PricePerUnit * sizeValue;
            }
            return result;
        }
    }
}
