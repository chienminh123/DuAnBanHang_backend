using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Admin
{
    public class ChiTietKiemKe
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int KiemKeId { get; set; }
        [Required]
        public int NguyenLieuId { get; set; }
        [Required]
        public float TonHeThong { get; set; }
        [Required]
        public float TonThucTe  { get; set; }
        [Required]
        public float ChenhLech { get; set; }
        public string? Note { get; set; }
        [ForeignKey("KiemKeId")]
        public KiemKe? KiemKe { get; set; }

        [ForeignKey("NguyenLieuId")]
        public NguyenLieu? NguyenLieu { get; set; }
    }
}
