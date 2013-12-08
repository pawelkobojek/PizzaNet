using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace PizzaNetControls.Validation
{
    public class StringValidation : ValidationRule
    {
        public StringValidation()
        {
            Regex = null;
            AllowEmpty = true;
            AllowWhiteSpace = true;
            MinLength = int.MinValue;
            MaxLength = int.MaxValue;
        }

        public bool AllowEmpty { get; set; }
        public bool AllowWhiteSpace { get; set; }
        public Regex Regex { get; set; }
        public bool IntegersOnly { get; set; }
        public int MinLength { get; set; }
        public int MaxLength { get; set; } 

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string str = (string)value;
            bool result = AllowEmpty || !String.IsNullOrEmpty(str);
            if (!result) return new ValidationResult(false, "String is null or empty");
            result = AllowWhiteSpace || !String.IsNullOrWhiteSpace(str);
            if (!result) return new ValidationResult(false, "String is null or whitespace");
            result = Regex==null || Regex.IsMatch(str);
            if (!result) return new ValidationResult(false, "String does not match regex");
            if (MinLength>str.Length || MaxLength<str.Length) return new ValidationResult(false, "String length is not in range");
            return new ValidationResult(true, "");
        }
    }
}
