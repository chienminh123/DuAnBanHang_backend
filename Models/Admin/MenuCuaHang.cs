using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Admin
{
    public class MenuCuaHang
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ShopId { get; set; }

        [Required]
        public int SanPhamId { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey("ShopId")]
        public CuaHang? CuaHang { get; set; }

        [ForeignKey("SanPhamId")]
        public SanPham? SanPham { get; set; }
    }
}