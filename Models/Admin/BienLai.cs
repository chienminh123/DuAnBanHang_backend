using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Admin
{
    public class BienLai
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string HanhDong { get; set; } // nhập kho ,xuất kho, xuất hủy
        [Required]
        public int CuaHangId { get; set; }
        
        public int? KhoXuatId { get; set; }
        public int? DoiTacId  { get; set; }
        [Required]
        public DateTime NgayThucHien { get; set; }
        [Required]
        public string TrangThai { get; set; }   // chờ xác nhận, thành công, hủy
        [ForeignKey("CuaHangId")]
        public CuaHang? CuaHang { get; set; }
        [ForeignKey("DoiTacId")]
        public DoiTac? DoiTac { get; set; }
    }
}
