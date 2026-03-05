namespace backend.DTOs.Admin
{
    public class KiemKeDTO
    {
        public int ShopId  { get; set; }
        public List<KiemKeItem> ChiTiet { get; set; }
    }

    public class KiemKeItem
    {
        public int NguyenLieuId { get; set; }
        public float TonThucTe { get; set; }
        public string? Note { get; set; }
    }
}
