using backend.Attribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Admin
{
    public class TaiKhoanNoiBo
    {
        [Key]
        public int TaiKhoanNoiBoId { get; set; }
        [Required]
        [Display(Name ="Tài khoản đăng nhập")]
        public string TenTaiKhoan { get; set; }
        [Required]
        [Display(Name ="Mật khẩu")]
        public string MatKhau { get; set; }
        [Required]
        public int ChucVuId { get; set; }
       
        public int? CuaHangId { get; set; }
        [Required]
        [StringLength(200)]
        public string TenNhanVien{ get; set; }
        [Required]
        public bool GioiTinh { get; set; }
        [Required]
        [VNPhone]
        public string Sdt { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string? Avatar { get; set; }
        [Required]
        public DateTime NgaySinh { get; set; }
        [Required]
        public DateTime NgayThamGia { get; set; }
        [Required]
        public bool IsActive { get; set; } = true;

        [ForeignKey("ChucVuId")]
        public ChucVu? ChucVu { get; set; }
        [ForeignKey("CuaHangId")]
        public CuaHang? CuaHang { get; set; }
        [NotMapped]
        public string ChucVuName { get; set; }

    }
}
