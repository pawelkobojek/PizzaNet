using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PizzaNetControls.Validation
{
    public class PasswordEqualRule : ValidationRule
    {
        public PasswordConfig PasswordConfig { get; set; }
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (PasswordConfig == null) return new ValidationResult(false, "No PasswordConfig set");
            string val = value as string;
            if (val == null) return new ValidationResult(false, "Value cannot be converted to string");
            string temp = PasswordConfig.Value;
            return new ValidationResult(String.Equals(val, temp) || (PasswordConfig.AllowEmpty && String.Equals(val, "")),"Incorrect password");
        }
    }
}
