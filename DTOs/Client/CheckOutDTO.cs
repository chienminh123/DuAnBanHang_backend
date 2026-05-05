namespace backend.DTOs.Client
{
    public class CheckOutDTO
    {
        public string TenNguoiNhan { get; set; }
        public string SdtNguoiNhan { get; set; }
        public string DiaChiGiaoHang { get; set; }
        public string PhuongThucThanhToan { get; set; }
        public string? GhiChuDonHang { get; set; }
        public double TongTienHang { get; set; }
        public double PhiGiaoHang { get; set; }
        public int ShopId { get; set; }
    }
}
