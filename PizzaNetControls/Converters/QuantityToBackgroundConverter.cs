﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PizzaNetControls.Converters
{
    [ValueConversion(typeof(decimal), typeof(Brush))]
    public class QuantityToBackgroundConverter : IValueConverter
    {
        private static System.Windows.Media.Color convertColor(System.Drawing.Color c)
        {
            return System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal? obj = value as decimal?;
            if (obj == null) return null;
            System.Windows.Media.Brush b = new SolidColorBrush(((obj ?? decimal.MaxValue) == 0) ? convertColor(System.Drawing.Color.LightSalmon) : convertColor(System.Drawing.Color.LightGreen));
            return (Brush)b;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}