
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;


namespace Bookstore.Common.Validation
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions.Select(e => e.StartsWith(".") ? e.ToLowerInvariant() : $".{e.ToLowerInvariant()}").ToArray(); ;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || !(value is IFormFile file))
            {
                return ValidationResult.Success;
            }


                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (String.IsNullOrEmpty(extension)||!_extensions.Contains(extension)) return new ValidationResult($"Invalid file extension. Allowed: {string.Join(", ", _extensions)}");

            return ValidationResult.Success;
        }

    }
}
