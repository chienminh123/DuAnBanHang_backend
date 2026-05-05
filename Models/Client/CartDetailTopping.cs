using backend.Models.Admin;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Client
{
    public class CartDetailTopping
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CartDetailId { get; set; }

        [Required] 
        public int ToppingSanPhamId { get; set; }

        [Required]
        public int SoLuong { get; set; }

        [ForeignKey("CartDetailId")]
        public CartDetail? CartDetail { get; set; }

        [ForeignKey("ToppingSanPhamId")] 
        public SanPham? SanPhamTopping { get; set; }
    }

    
}

