using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PizzaNetControls.Converters
{
    [ValueConversion(typeof(int), typeof(int))]
    public class IntModifierConverter : IValueConverter
    {
        public int Modifier { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int? obj = value as int?;
            if (obj == null) return null;
            return obj.Value + Modifier;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
