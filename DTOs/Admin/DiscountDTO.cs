using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Admin
{
    public class DiscountDTO
    {
        [Required]
        public string? Code { get; set; }
        [Required]
        public string? LoaiMa { get; set; } = "Persent";
        [Required]
        public int DiscountValue { get; set; }
        [Required]
        public float? MaxValue { get; set; }
        [Required]
        public int? SoLuong { get; set; }
        [Required]
        public DateTime? NgayBatDau { get; set; }
        [Required]
        public DateTime? NgayKetThuc { get; set; }
    }
}
