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
        public event EventHandler<ValidationEventArgs> Validation;
        public PasswordConfig PasswordConfig { get; set; }
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (PasswordConfig == null) return new ValidationResult(false, "No PasswordConfig set");
            string val = value as string;
            if (val == null) return new ValidationResult(false, "Value cannot be converted to string");
            string temp = PasswordConfig.Value;
            bool result = String.Equals(val, temp) || (PasswordConfig.AllowEmpty && String.Equals(val, ""));
            if (Validation != null)
                Validation(this, new ValidationEventArgs { Result = result });
            return new ValidationResult(result,"Incorrect password");
        }

        public class ValidationEventArgs : EventArgs
        {
            public bool Result { get; set; }
        }
    }
}
