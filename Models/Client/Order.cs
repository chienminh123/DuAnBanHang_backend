using backend.Models.Admin;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Client
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string MaDonHang { get; set; }

        [Required]
        [StringLength(50)]
        public string LoaiDonHang { get; set; } 

        public int? KhachHangId { get; set; } 

        public int? TaiKhoanNoiBoId { get; set; } 

        [Required]
        public int ShopId { get; set; }

        //  THÔNG TIN GIAO HÀNG ONLINE 
        [StringLength(100)]
        public string? TenNguoiNhan { get; set; }

        [StringLength(20)]
        public string? SdtNguoiNhan { get; set; }

        [StringLength(500)]
        public string? DiaChiGiaoHang { get; set; }

        public double PhiGiaoHang { get; set; } = 0;

        [Required]
        public double TongTienHang { get; set; }

        public double TienGiamGia { get; set; } = 0;

        public double DiemSuDung { get; set; } = 0; 
        public double DiemCongThem { get; set; } = 0; 

        [Required]
        public double ThanhTien { get; set; } 

        [Required]
        [StringLength(50)]
        public string PhuongThucThanhToan { get; set; } 

        [Required]
        public bool IsThanhToan { get; set; } = false;

        [Required]
        [StringLength(50)]
        public string TrangThaiDonHang { get; set; } = "CHO_XAC_NHAN"; 

        [StringLength(500)]
        public string? GhiChu { get; set; }

        [Required]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [ForeignKey("KhachHangId")]
        public TaiKhoanKhachHang? KhachHang { get; set; }

        [ForeignKey("TaiKhoanNoiBoId")]
        public TaiKhoanNoiBo? TaiKhoanPOS { get; set; }

        [ForeignKey("ShopId")]
        public CuaHang? CuaHang { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}