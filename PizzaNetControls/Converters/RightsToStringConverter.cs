using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PizzaNetControls.Converters
{

    [ValueConversion(typeof(int), typeof(string))]
    public class RightsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int? obj = value as int?;
            if (obj == null) return null;
            switch (obj)
            {
                case 1:
                    return "Customer";
                case 2:
                    return "Employee";
                case 3:
                    return "Admin";
                default:
                    throw new NotSupportedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string obj = value as string;
            if (obj == null)
                return null;

            switch (obj)
            {
                case "Customer":
                    return 1;
                case "Employee":
                    return 2;
                case "Admin":
                    return 3;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
