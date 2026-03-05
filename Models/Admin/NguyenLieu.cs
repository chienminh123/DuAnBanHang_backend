using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Admin
{
    public class NguyenLieu
    {
        [Key]
        public int NguyenLieuId { get; set; }
        public int? DoiTacId { get; set; }
        [Required]
        public string NguyenLieuName { get; set; }
        [Required]
        public string DonVi { get; set; }       //kg,g,lm,cái 
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaNhap { get; set; }
        [Required]
        public int TheLoaiId { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("TheLoaiId")]
        public TheLoai? TheLoai { get; set; }
        [ForeignKey("DoiTacId")]
        public DoiTac? DoiTac { get; set; }
    }
}
