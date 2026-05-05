using System.ComponentModel.DataAnnotations;

namespace backend.Models.Admin
{
    public class ChamCong
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TaiKhanNoiBoId { get; set; }
        [Required]
        public int CaLamViecId { get; set; }

        public DateTime Ngay { get; set; }
        public DateTime? GioVao { get; set; }
        public DateTime? GioRa { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string TrangThai { get; set; } // Đúng giờ, Đi muộn, Về sớm
        public string GhiChu { get; set; }
    }
}
