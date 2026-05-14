using System.ComponentModel.DataAnnotations;

namespace backend.Models.Admin
{
    public class ChotCa
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ShopId { get; set; }
        [Required]
        public DateTime ThoiGianMo { get; set; }
        public DateTime? ThoiGianDong { get; set; }

        public double? TienDauCa { get; set; }
        [Required]
        public double TongThuTienMat { get; set; }
        [Required]
        public double TongThuVNPAY { get; set; }

        public double? TienMatThucTe { get; set; }
        public double? VnpayThucTe { get; set; }
        [Required]
        public string TrangThai { get; set; } // "DANG_MO" hoặc "DA_CHOT"

        public string? GhiChu { get; set; }
    }
}