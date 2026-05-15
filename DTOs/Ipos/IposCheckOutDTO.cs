namespace backend.DTOs.Ipos
{
    public class IposCheckOutDTO
    {
        public int ShopId { get; set; }
        public string PhuongThucThanhToan { get; set; } // "TIEN_MAT", "VNPay"
        public double ThanhTien { get; set; }
        public double TongTienHang { get; set; }
        public double TienGiamGia { get; set; }
        public double DiemSuDung { get; set; }
        public string? SdtKhachHang { get; set; }
        public string? GhiChu { get; set; }
        public int? KhachHangId { get; set; }


        // Danh sách món ăn gửi từ App lên
        public List<IposOrderItemDTO> Items { get; set; } = new List<IposOrderItemDTO>();
    }

    public class IposOrderItemDTO
    {
        public int SanPhamId { get; set; }
        public int SoLuong { get; set; }
        public double DonGia { get; set; } // Giá của Size
        public bool IsKhongNau { get; set; }
        public int? RauCuNguyenLieuId { get; set; }
        public string? GhiChu { get; set; }

        // Danh sách Topping của món này
        public List<IposOrderToppingDTO> Toppings { get; set; } = new List<IposOrderToppingDTO>();
    }

    public class IposOrderToppingDTO
    {
        public int ToppingSanPhamId { get; set; }
        public int SoLuong { get; set; }
        public double GiaTopping { get; set; }
    }
}

