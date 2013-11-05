using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PizzaNetControls.Validation
{
    public class NumberValidation : ValidationRule
    {
        public NumberValidation()
        {
            AllowNegative = AllowZero = AllowPositive = true;
            IntegersOnly = false;
        }

        public bool AllowNegative { get; set; }
        public bool AllowZero { get; set; }
        public bool AllowPositive { get; set; }
        public bool IntegersOnly { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string str = (string)value;
            return (IntegersOnly) ? ValidateInteger(str) : ValidateDouble(str);
        }

        private ValidationResult ValidateInteger(string value)
        {
            int d = 0;
            bool result = int.TryParse(value, out d);
            if (!AllowNegative)
                result = result && (d >= 0);
            if (!AllowPositive)
                result = result && (d <= 0);
            if (!AllowZero)
                result = result && (d != 0);
            return new ValidationResult(result, null);
        }

        private ValidationResult ValidateDouble(string value)
        {
            if (value.Contains(','))
                return new ValidationResult(false, "Wrong digit separator");
            double d = 0;
            bool result = double.TryParse(value.Replace('.', ','), out d);
            if (!AllowNegative)
                result = result && (d >= 0);
            if (!AllowPositive)
                result = result && (d <= 0);
            if (!AllowZero)
                result = result && (d != 0);
            return new ValidationResult(result, null);
        }
    }
}
