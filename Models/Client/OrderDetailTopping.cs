using backend.Models.Admin;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Client
{
    public class OrderDetailTopping
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderDetailId { get; set; }

        [Required]
        public int ToppingSanPhamId { get; set; } 

        [Required]
        public int SoLuong { get; set; }

        [Required]
        public double GiaTopping { get; set; }

        [ForeignKey("OrderDetailId")]
        public OrderDetail? OrderDetail { get; set; }

        [ForeignKey("ToppingSanPhamId")] 
        public SanPham? SanPhamTopping { get; set; }
    }
}