using backend.Attribute;
using backend.Models.Admin;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Client
{
    public class TaiKhoanKhachHang
    {
        [Key]
        public int KhachHangId { get; set; }
        [Required]
        [StringLength(200,ErrorMessage ="Tới đa 200 kí tự")]
        public string TenKhachHang { get; set; }
        [Required]
        [VNPhone]
        [Display(Name ="Số điện thoại")]
        public string Sdt { get; set; }
        [Required]
        [Display(Name ="Email")]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Display(Name ="Mật khẩu")]
        public string MatKhau { get; set; }
        [Required]
        public int ChucVuId { get; set; }
        [Required]
        public DateTime NgaySinh { get; set; }
        [Required]
        public DateTime NgayThamGia { get; set; }
        [Required]
        public double TichDiem { get; set; }

        [ForeignKey("ChucVuId")]
        public ChucVu ChucVu { get; set; }
        [NotMapped]
        public string ChucVuName { get; set; }
    }
}
