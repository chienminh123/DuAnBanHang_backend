using System.ComponentModel.DataAnnotations;

namespace backend.Models.Admin
{
    public class CaLamViec
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string TenCa { get; set; }
        [Required]
        public TimeSpan GioBatDau { get; set; }
        [Required]
        public TimeSpan GioKetThuc { get; set; }

    }
}
