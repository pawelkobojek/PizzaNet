using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace PizzaNetControls
{
    [ValueConversion(typeof(PriceData), typeof(String))]
    public class PriceDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            PriceData obj = value as PriceData;
            if (obj == null) return null;
            return "Price: "+obj.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
