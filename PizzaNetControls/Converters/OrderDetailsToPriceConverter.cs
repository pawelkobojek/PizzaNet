using PizzaNetControls.Common;
using PizzaNetDataModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PizzaNetControls.Converters
{
    [ValueConversion(typeof(OrderDetail), typeof(String))]
    public class OrderDetailsToPriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            OrderDetail obj = value as OrderDetail;
            if (obj == null) return null;
            return PriceCalculator.CalculatePrice(obj).ToString("F2");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
