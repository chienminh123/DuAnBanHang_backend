using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Admin
{
    public class CauHinhLuongDTO
    {
        [Required]
        public int TaiKhoanNoiBoId { get; set; }

        [Required]
        [StringLength(50)]
        public string LoaiLuong { get; set; } 

        [Required]
        public double MucLuong { get; set; }

        public double PhuCapAnTrua { get; set; } = 0;
        public double PhuCapXangXe { get; set; } = 0;
        public double PhuCapChuyenCan { get; set; } = 0;
    }
}