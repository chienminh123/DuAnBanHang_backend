namespace backend.DTOs.Admin
{
    public class Check_InDTO
    {
        public int TaiKhoanNoiBoId { get; set; }
        public int ShopId { get; set; }
        public int CaLamViecId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
