using backend.Models.Admin;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Client
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int SanPhamId { get; set; }
        public int? RauCuNguyenLieuId { get; set; }

        [Required]
        public int SoLuong { get; set; }

        [Required]
        public double DonGia { get; set; }
        [Required]
        public bool IsKhongNau { get; set; }

        [Required]
        public double ThanhTien { get; set; } 

        [StringLength(200)]
        public string? GhiChu { get; set; }

        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        [ForeignKey("RauCuNguyenLieuId")]
        public NguyenLieu? RauCu { get; set; }

        public ICollection<OrderDetailTopping> OrderDetailToppings { get; set; } = new List<OrderDetailTopping>();  
    }
}