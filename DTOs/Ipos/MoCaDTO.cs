using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Ipos
{
    public class MoCaDTO
    {
        [Required(ErrorMessage = "ShopId là bắt buộc")]
        public int ShopId { get; set; }

        public double? TienDauCa { get; set; }
    }
}
