using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Oracle_WEB_BTL.Models;

namespace Oracle_WEB_BTL.ValidationAttributes
{
    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = value as string;
            if (string.IsNullOrWhiteSpace(email))
            {
                return new ValidationResult("Email là bắt buộc.");
            }

            var employee = (Oracle_WEB_BTL.Models.Nhanvien)validationContext.ObjectInstance;
            decimal employeeId = employee.Manv;

            var _context = validationContext.GetService(typeof(OracleContext)) as OracleContext;

            var existingEmployee = _context.Nhanviens
                .FirstOrDefault(e => e.Email == email && e.Manv != employeeId);

            if (existingEmployee != null)
            {
                return new ValidationResult("Email đã tồn tại trong hệ thống.");
            }

            return ValidationResult.Success;
        }
    }
}
