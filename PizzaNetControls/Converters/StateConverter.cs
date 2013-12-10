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
    [ValueConversion(typeof(StateDTO), typeof(String))]
    public class StateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            StateDTO obj = value as StateDTO;
            if (obj == null) return null;

            //TODO ToString() dla StateDTO
            return obj.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
