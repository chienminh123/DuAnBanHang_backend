using backend.Attribute;
using System.ComponentModel.DataAnnotations;

namespace backend.Models.Admin
{
    public class CuaHang
    {
        [Key]
        public int ShopId { get; set; }
        [Required]
        public string ShopName { get; set; }
        [Required]
        public string ShopAddress { get; set; }
        [Required]
        [VNPhone]
        public string ShopPhone { get; set; }
        [Required]
        public string ShopCity { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int BanKinhChoPhep { get; set; } = 50;
    }
}
