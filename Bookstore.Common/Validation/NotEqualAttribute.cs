using System.ComponentModel.DataAnnotations;
namespace Bookstore.Common.Validation
{
    public class NotEqualAttribute : ValidationAttribute
    {
        private readonly string _otherProperty;

        public NotEqualAttribute(string otherProperty)
        {
            _otherProperty = otherProperty;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            var currentValue = value.ToString();
            var otherPropInfo = validationContext.ObjectType.GetProperty(_otherProperty);

            if (otherPropInfo == null) return new ValidationResult($"Unknown property: {_otherProperty}");

            var otherValue = otherPropInfo.GetValue(validationContext.ObjectInstance)?.ToString();

            if (string.Equals(currentValue, otherValue, StringComparison.OrdinalIgnoreCase)) return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} must not match {_otherProperty}.");

            return ValidationResult.Success;
        }
    }
}

