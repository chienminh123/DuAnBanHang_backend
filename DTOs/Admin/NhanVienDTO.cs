namespace backend.DTOs.Admin
{
    public class NhanVienDTO
    {
        public string name { get; set; }
        public string sdt { get; set; }
        public string email { get; set; }
        public DateTime NgaySinh { get; set; }
        public bool GioiTinh { get; set; }
        public string MatKhauMoi { get; set; }
        public IFormFile? AvatarUpload { get; set; }
}
}
