using System.ComponentModel.DataAnnotations;
using System.Linq;
using Oracle_WEB_BTL.Models;

namespace Oracle_WEB_BTL.ValidationAttributes
{
    public class UniquePhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var context = (OracleContext)validationContext.GetService(typeof(OracleContext));
            var employeeId = (decimal)validationContext.ObjectType.GetProperty("Manv").GetValue(validationContext.ObjectInstance);

            var phoneExists = context.Nhanviens.Any(e => e.Dienthoai == value.ToString() && e.Manv != employeeId);

            if (phoneExists)
            {
                return new ValidationResult("Số điện thoại đã tồn tại trong hệ thống.");
            }

            return ValidationResult.Success;
        }
    }
}
