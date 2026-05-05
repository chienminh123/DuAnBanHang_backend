using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Client
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int KhachHangId { get; set; } 

        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        [ForeignKey("KhachHangId")]
        public TaiKhoanKhachHang? KhachHang { get; set; }

        public ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
    }
}