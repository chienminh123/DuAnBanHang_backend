using System.ComponentModel.DataAnnotations;

namespace backend.Models.Admin
{
    public class LichLamViec
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int TaiKhoanNoiBoId { get; set; }
        [Required]
        public int CaLamViecId { get; set; }
        [Required]
        public DateTime NgayLamViec { get; set; }
        [Required]
        public int ShopId { get; set; }

    }
}
