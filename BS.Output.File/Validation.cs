using System.Windows.Controls;

namespace BS.Output.File
{

  public class NotNullOrEmptyValidationRule : ValidationRule
  {

    public NotNullOrEmptyValidationRule()
    {
      ValidatesOnTargetUpdated = true;
    }

    public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
    {
      if (string.IsNullOrEmpty((string)value))
      {
        return new ValidationResult(false, string.Empty);
      }
      else
      {
        return ValidationResult.ValidResult;
      }
    }
  }

}
