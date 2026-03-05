using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class LoginDTO
    {
        [Required]
        public string TenTaiKhoan { get; set; }
        [Required]
        public string MatKhau { get; set; }

    }
}
