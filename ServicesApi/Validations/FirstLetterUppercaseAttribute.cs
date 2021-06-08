#nullable enable
namespace ServicesApi.Validations
{
    using System.ComponentModel.DataAnnotations;

    public class FirstLetterUppercaseAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var firstLetter = value.ToString()?[0].ToString();
            return firstLetter != firstLetter?.ToUpper() ? new ValidationResult("La primera letra debe ser mayúscula") : ValidationResult.Success;
        }
    }
}