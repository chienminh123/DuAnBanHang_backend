using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace backend.Models.Admin
{
    public class ChucVu
    {
        [Key]
        public int ChucVuId { get; set; }
        [Required]
        [StringLength(100)]
        public string ChucVuName { get; set; }
        [Required]
        [StringLength(1000,ErrorMessage ="Mô tả chức vụ k quá 1000 từ")]
        public string ChucVuDescription { get; set; }
        [JsonIgnore]
        public List<TaiKhoanNoiBo>? taiKhoanNoiBos  { get; set; }

    }
}
