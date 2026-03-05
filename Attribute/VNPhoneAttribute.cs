using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace backend.Attribute
{
    public class VNPhoneAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success; 

            string phone = value.ToString()!;

            // Regex kiểm tra 10 số, bắt đầu bằng 03, 05, 07, 08, 09
            string pattern = @"^(0)(3|5|7|8|9)[0-9]{8}$";

            if (Regex.IsMatch(phone, pattern))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Số điện thoại không đúng định dạng Việt Nam (10 số, bắt đầu bằng 03,05,07,08,09).");
        }
    }
}
