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
    

    }
}
