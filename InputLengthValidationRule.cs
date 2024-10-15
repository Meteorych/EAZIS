using System.Windows.Controls;

namespace EAZIS1
{
    public class InputLengthValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object? value, System.Globalization.CultureInfo cultureInfo)
        {
            if (!int.TryParse(value as string, out int inputLength))
                return new ValidationResult(false, "Please enter a number between 1 and 50.");
            return inputLength is >= 1 and <= 50 ? ValidationResult.ValidResult : new ValidationResult(false, "Please enter a number between 1 and 50.");
        }
    }
}