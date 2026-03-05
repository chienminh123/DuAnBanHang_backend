using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Admin
{
    public class SanPham
    {
        [Key]
        public int SanPhamId { get; set; }

        [Required]
        public string SanPhamName { get; set; }
        [Required]
        public int TheLoaiId { get; set; }
        [Required]
        public string Size { get; set; }
        [Required]
        public string? MoTa { get; set; }
        [Required]
        public string HinhAnh { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaBan { get; set; }
        public int NguyenLieuId { get; set; }
        [Required]
        public bool IsActive { get; set; }

        [ForeignKey("TheLoaiId")]
        public TheLoai? TheLoai { get; set; }
        [ForeignKey("NguyenLieuId")]
        public NguyenLieu? NguyenLieu { get; set; }
    }
}
