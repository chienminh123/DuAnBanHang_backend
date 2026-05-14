namespace backend.DTOs.Admin
{
    public class BaoCaoDTO
    {
        public List<int>? ShopIds { get; set; } = new List<int>();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
