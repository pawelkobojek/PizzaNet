using PizzaNetDataModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using PizzaNetCommon.DTOs;

namespace PizzaNetControls.Converters
{
    [ValueConversion(typeof(IEnumerable<OrderIngredientDTO>), typeof(String))]
    public class IngredientsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var list = value as IEnumerable<OrderIngredientDTO>;
            if (list == null) return null;
            StringBuilder sb = new StringBuilder();
            bool first=true;
            foreach (var e in list)
            {
                if (!first) sb.Append(" / ");
                sb.Append(e.Name);
                first = false;
            }
            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
