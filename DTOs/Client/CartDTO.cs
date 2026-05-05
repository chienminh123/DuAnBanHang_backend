using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Client
{
    public class CartDTO
    {
        public int SanPhamId { get; set; }
        public int SoLuong { get; set; }
        public bool IsKhongNau { get; set; }
        public int? RauCuNguyenLieuId { get; set; } 
        public string? GhiChu { get; set; }
        public List<Topping> Toppings { get; set; } = new List<Topping>();
    }

    public class Topping
    {
        public int ToppingSanPhamId { get; set; } 
        public int SoLuong { get; set; }
    }
}
