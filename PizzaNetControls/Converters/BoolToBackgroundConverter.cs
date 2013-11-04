using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PizzaNetControls.Converters
{
    [ValueConversion(typeof(bool), typeof(Brush))]
    public class BoolToBackgroundConverter : IValueConverter
    {
        private static System.Windows.Media.Color convertColor(System.Drawing.Color c)
        {
            return System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var obj = value as bool?;
            if (obj == null) return null;
            var val = obj ?? false;
            System.Windows.Media.Brush b = new SolidColorBrush((!val) ? convertColor(System.Drawing.Color.LightSalmon) : convertColor(System.Drawing.Color.LightGreen));
            return (Brush)b;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
