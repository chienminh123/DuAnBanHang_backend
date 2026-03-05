using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Admin
{
    public class ChiTietBienLai
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int BienLaiId { get; set; }
        [Required]
        public int NguyenLieuId { get; set; }
        [Required]
        public float Soluong { get; set; }
        [StringLength(1000)]
        public string? GhiChu { get; set; }

        [ForeignKey("NguyenLieuId")]
        public NguyenLieu? NguyenLieu { get; set; }
        [ForeignKey("BienLaiId")] 
        public BienLai? BienLai { get; set; }
    }
}
