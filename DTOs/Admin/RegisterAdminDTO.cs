using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Admin
{
    public class RegisterAdminDTO
    {
        [Required]
        public string TenTaiKhoan { get; set; }
        [Required]
        public string MatKhau { get; set; }
        [Required]
        public string TenNhanVien { get; set; }
        public string Email { get; set; }
        public string Sdt { get; set; }
        public bool GioiTinh { get; set; }
        public DateTime NgaySinh { get; set; }
        [Required]
        public int ChucVuId { get; set; }
        public int? CuaHangId { get; set; }

        public IFormFile? AvatarUpload { get; set; }
    }
}
