using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Admin
{
    public class TonKho
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ShopId { get; set; } // cửa hàng nào
        [Required]
        public int NguyenLieuId { get; set; }
        [Required]
        public float SoLuong { get; set; }
        [ForeignKey("NguyenLieuId")]
        public NguyenLieu? NguyenLieu { get; set; }
        
        [ForeignKey("ShopId")]
        public CuaHang? CuaHang { get; set; }
    }
}
