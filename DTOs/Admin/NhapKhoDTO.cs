namespace backend.DTOs.Admin
{
    public class NhapKhoDTO
    {
        public int? KhoNhapId { get; set; } // Nhập vào kho nào
        public int? KhoXuatId { get; set; } // Nhập từ ai
        public int? DoiTacId { get; set; }
        public string HanhDong { get; set; } // "NhapKho"
      


        // Danh sách các món cần nhập
        public List<NguyenLieuNhapItem> Items { get; set; }
    }

    public class NguyenLieuNhapItem
        {
            public int NguyenLieuId { get; set; }
            public string NguyenLieuName { get; set; }
            public string DonVi { get; set; } // kg, hop...
            public int TheLoaiId { get; set; }
            public float SoLuong { get; set; }
            public decimal GiaNhap { get; set; }
            public DateTime  NgaySanXuat { get; set; }
            public DateTime HanSuDung { get; set; }
        public string? GhiChu { get; set; }
        } 
    
}
