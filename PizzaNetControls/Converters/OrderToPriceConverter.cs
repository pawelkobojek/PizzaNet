using PizzaNetControls.Common;
using PizzaNetDataModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using PizzaNetCommon.DTOs;

namespace PizzaNetControls.Converters
{
    [ValueConversion(typeof(OrderDTO), typeof(String))]
    public class OrderToPriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            OrderDTO obj = value as OrderDTO;
            if (obj == null) return null;
            return PriceCalculator.CalculatePrice(obj).ToString("F2");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
