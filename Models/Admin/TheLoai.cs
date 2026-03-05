using System.ComponentModel.DataAnnotations;

namespace backend.Models.Admin
{
    public class TheLoai
    {
        [Key]
        public int TheLoaiId { get; set; }
        [Required]
        public string TheLoaiName { get; set; }
        public List<SanPham> sanPhams { get; set; }
    }
}
