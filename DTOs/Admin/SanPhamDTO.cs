using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Admin
{
    public class SanPhamDTO
    {
        [Required]
        public string SanPhamName { get; set; }

        [Required]
        public int TheLoaiId { get; set; }
        public int NguyenLieuId { get; set; }
        [Required]
        public string Size { get; set; }
        [Required]
        public string MoTa { get; set; }

        [Required]
        public decimal GiaBan { get; set; }
        public bool IsActive { get; set; }
        public IFormFile? HinhAnhUpload { get; set; }
    }
}
