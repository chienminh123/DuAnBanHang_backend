using backend.Models.Admin;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Client
{
    public class CartDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CartId { get; set; }

        [Required]
        public int SanPhamId { get; set; }
        public int? RauCuNguyenLieuId { get; set; }

        [Required]
        public int SoLuong { get; set; }
        [Required]
        public bool IsKhongNau { get; set; } = false;

        [StringLength(200)]
        public string? GhiChu { get; set; } 

        [ForeignKey("CartId")]
        public Cart? Cart { get; set; }
        [ForeignKey("RauCuNguyenLieuId")] 
        public NguyenLieu? RauCu { get; set; }

        public ICollection<CartDetailTopping> Toppings { get; set; } = new List<CartDetailTopping>();
    }
}