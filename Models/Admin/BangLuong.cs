using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Admin
{
    public class BangLuong
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TaiKhoanNoiBoId { get; set; }

        [Required]
        public int Thang { get; set; } 

        [Required]
        public int Nam { get; set; } 

        public double TongGioLam { get; set; } 
        public double TongNgayLam { get; set; } 

        public double TienPhatDiMuon { get; set; } 

        [Required]
        public double TongTienNhan { get; set; }
        public double PhuCapAnTrua { get; set; } = 0;
        public double PhuCapXangXe { get; set; } = 0;
        public double PhuCapChuyenCan { get; set; } = 0;
        public double TongPhuCap { get; set; } = 0;

        [Required]
        [StringLength(50)]
        public string TrangThai { get; set; } = "CHUA_THANH_TOAN"; // "CHUA_THANH_TOAN", "DA_THANH_TOAN"

        [ForeignKey("TaiKhoanNoiBoId")]
        public TaiKhoanNoiBo? TaiKhoanNoiBo { get; set; }
    }
}