using System;
using System.ComponentModel.DataAnnotations;

namespace Oracle_WEB_BTL.ValidationAttributes
{
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
            ErrorMessage = $"Tuổi phải lớn hơn hoặc bằng {_minimumAge}.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime birthDate)
            {
                var currentDate = DateTime.Now;

                var age = currentDate.Subtract(birthDate).TotalDays / 365.25;

                if (age >= _minimumAge)
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(ErrorMessage);
        }
    }
}
