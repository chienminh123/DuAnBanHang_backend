using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Admin
{
    public class KiemKe
    {
        [Key]
        public int KiemKeId { get; set; }
        [Required]
        public int ShopId { get; set; }
        [Required]
        public DateTime NgayThucHien { get; set; } 
        [ForeignKey("ShopId")]
        public CuaHang? CuaHang { get; set; }
    }
}
