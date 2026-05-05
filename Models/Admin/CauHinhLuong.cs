using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Admin
{
    public class CauHinhLuong
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TaiKhoanNoiBoId { get; set; }

        [Required]
        [StringLength(50)]
        public string LoaiLuong { get; set; } // "THEO_GIO" hoặc "CO_DINH"

        [Required]
        public double MucLuong { get; set; } // Ví dụ: 25000 (nếu theo giờ), hoặc 8000000 (nếu cố định)
        public double PhuCapAnTrua { get; set; } = 0;
        public double PhuCapXangXe { get; set; } = 0;
        public double PhuCapChuyenCan { get; set; } = 0;

        [ForeignKey("TaiKhoanNoiBoId")]
        public TaiKhoanNoiBo? TaiKhoanNoiBo { get; set; }
    }
}