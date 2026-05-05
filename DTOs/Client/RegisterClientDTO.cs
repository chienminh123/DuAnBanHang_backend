using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Client
{
    public class RegisterClientDTO
    {
        public string HoTen { get; set; }
        public string sdt { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string MatKhau { get; set; }
        public DateTime NgaySinh { get; set; }
        public string? OtpCode { get; set; } 
    }
}
