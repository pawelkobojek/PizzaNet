using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace PizzaNetControls.Converters
{
    [ValueConversion(typeof(GridViewColumn), typeof(int))]
    public class StarWidthGridViewColumnConverter : IValueConverter
    {
        public int Modifier { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            GridViewColumn gv = value as GridViewColumn;
            return gv.ActualWidth + Modifier;// this is to take care of margin/padding
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
