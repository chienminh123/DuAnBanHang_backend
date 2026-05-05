using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Admin
{
    public class ShiftDTO
    {
        [Required]
        public string TenCa { get; set; }
        [Required]
        public TimeSpan GioBatDau { get; set; }
        [Required]
        public TimeSpan GioKetThuc { get; set; }
    }
}
