using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Admin
{
    public class ScheduleDTO
    {
        [Required]
        public int TaiKhoanNoiBoId { get; set; }
        [Required]
        public int CaLamViecId { get; set; }
        [Required]
        public DateTime NgayLamViec { get; set; }
        [Required]
        public int ShopId { get; set; }
        public List<int> CaLamViecIds { get; set; } = new List<int>();
    }
}
