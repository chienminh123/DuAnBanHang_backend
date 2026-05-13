using backend.Data;
using backend.Models.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.Ipos
{
    [Route("api/ipos/[controller]")]
    [ApiController]
    public class IposCheckOutController : ControllerBase
    {
        private readonly ShopContext _context;

        public IposCheckOutController(ShopContext context)
        {
            _context = context;
        }

        [HttpGet("pos-history/{shopId}")]
        public async Task<IActionResult> GetPosHistory(int shopId)
        {
            var today = DateTime.Today; 

            var orders = await _context.Order
                .Where(o => o.ShopId == shopId
                         && o.NgayTao.Date == today 
                         && (o.TrangThaiDonHang == "DA_THANH_TOAN" || o.TrangThaiDonHang == "HOAN_THANH" || o.TrangThaiDonHang == "PAID"))
                .OrderByDescending(o => o.NgayTao)
                .Select(o => new {
                    o.Id,
                    o.MaDonHang,
                    NgayTao = o.NgayTao.ToString("HH:mm - dd/MM/yyyy"), 
                    o.TongTienHang,
                    o.PhuongThucThanhToan
                })
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("web-orders-preparing/{shopId}")]
        public async Task<IActionResult> GetPreparingWebOrders(int shopId)
        {
            var orders = await _context.Order
                .Where(o => o.ShopId == shopId && o.TrangThaiDonHang == "DANG_CHUAN_BI")
                .OrderByDescending(o => o.NgayTao)
                .Select(o => new {
                    o.Id,
                    o.MaDonHang,
                    o.TongTienHang
                })
                .ToListAsync();

            return Ok(orders);
        }

        [HttpPost("ipos-checkout")]
        public async Task<IActionResult> IposCheckout([FromBody] backend.DTOs.Ipos.IposCheckOutDTO request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Tạo đơn hàng (Mua tại quầy thì trạng thái là DA_THANH_TOAN luôn)
                var newOrder = new backend.Models.Client.Order
                {
                    MaDonHang = "POS" + DateTime.Now.Ticks.ToString(),
                    LoaiDonHang = "AT_STORE",
                    ShopId = request.ShopId,
                    NgayTao = DateTime.Now,
                    TrangThaiDonHang = "DA_THANH_TOAN",
                    PhuongThucThanhToan = request.PhuongThucThanhToan,
                    TongTienHang = request.TongTien,
                    ThanhTien = request.TongTien,
                    KhachHangId = request.KhachHangId,
                    SdtNguoiNhan = request.SdtKhachHang,
                    GhiChu = request.GhiChu
                };

                _context.Order.Add(newOrder);
                await _context.SaveChangesAsync();

                //  Duyệt qua từng món ăn Android gửi lên
                foreach (var item in request.Items)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = newOrder.Id,
                        SanPhamId = item.SanPhamId,
                        SoLuong = item.SoLuong,
                        DonGia = item.DonGia,
                        IsKhongNau = item.IsKhongNau,
                        GhiChu = item.GhiChu,
                        RauCuNguyenLieuId = item.RauCuNguyenLieuId == 0 ? null : item.RauCuNguyenLieuId,
                        ThanhTien = item.DonGia * item.SoLuong
                    };
                    _context.OrderDetail.Add(orderDetail);
                    await _context.SaveChangesAsync();

                    //  Duyệt Topping của món đó
                    if (item.Toppings != null && item.Toppings.Any())
                    {
                        foreach (var t in item.Toppings)
                        {
                            var orderTopping = new backend.Models.Client.OrderDetailTopping
                            {
                                OrderDetailId = orderDetail.Id,
                                ToppingSanPhamId = t.ToppingSanPhamId,
                                SoLuong = t.SoLuong,
                                GiaTopping = t.GiaTopping
                            };
                            _context.OrderDetailTopping.Add(orderTopping);
                        }
                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();
                await XuLySauKhiBanXong(newOrder.Id, request.ShopId);
                return Ok(new { message = "Thanh toán thành công!", orderId = newOrder.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Lỗi hệ thống IPOS: " + ex.Message);
            }
        }

        [HttpGet("pos-order-details/{orderId}")]
        public async Task<IActionResult> GetPosOrderDetails(int orderId)
        {
            var order = await _context.Order
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.RauCu)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.OrderDetailToppings)
                        .ThenInclude(odt => odt.SanPhamTopping)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return NotFound("Không tìm thấy đơn hàng.");

            var spIds = order.OrderDetails.Select(od => od.SanPhamId).ToList();
            var sanPhams = await _context.sanPhams.Where(s => spIds.Contains(s.SanPhamId)).ToListAsync();

            var result = new
            {
                order.MaDonHang,
                order.NgayTao,
                order.TongTienHang,
                Items = order.OrderDetails.Select(od => {
                    var spChinh = sanPhams.FirstOrDefault(s => s.SanPhamId == od.SanPhamId);
                    return new
                    {
                        TenMon = spChinh?.SanPhamName ?? "Món ăn",
                        Size = spChinh?.Size ?? "M",
                        od.SoLuong,
                        od.ThanhTien,
                        RauCu = od.RauCu?.NguyenLieuName,
                        od.IsKhongNau,
                        od.GhiChu,
                        Toppings = od.OrderDetailToppings.Select(t => new {
                            TenTopping = t.SanPhamTopping?.SanPhamName,
                            t.SoLuong,
                            t.GiaTopping
                        })
                    };
                })
            };
            return Ok(result);
        }

        private async Task XuLySauKhiBanXong(int orderId, int shopId)
        {
            var order = await _context.Order
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.OrderDetailToppings)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return;

            // tích điểm 10k=1d
            if (order.KhachHangId != null && order.KhachHangId > 0)
            {
                var khach = await _context.taiKhoanKhachHang.FindAsync(order.KhachHangId);
                if (khach != null)
                {
                    int diemCong = (int)(order.ThanhTien / 10000); // 10k = 1 điểm
                    khach.TichDiem += diemCong;
                }
            }

            // tru kho
            var khoShop = await _context.tonKhos.Where(k => k.ShopId == shopId).ToListAsync();
            if (!khoShop.Any())
            {
                Console.WriteLine($"[CẢNH BÁO TỒN KHO] Cửa hàng ShopId = {shopId} chưa được nhập bất kỳ hàng hóa nào!");
            }
            foreach (var item in order.OrderDetails)
            {

                var spChinh = _context.sanPhams.FirstOrDefault(s => s.SanPhamId == item.SanPhamId);
                if (spChinh != null && spChinh.NguyenLieuId != null)
                {
                    var khoMonChinh = khoShop.FirstOrDefault(k => k.NguyenLieuId == spChinh.NguyenLieuId);

                    if (khoMonChinh != null)
                    {
                        float dinhMucGam = 60;
                        string sizeStr = spChinh.Size?.Trim().ToUpper();

                        if (sizeStr == "S") dinhMucGam = 40;
                        else if (sizeStr == "M") dinhMucGam = 60;
                        else if (sizeStr == "L") dinhMucGam = 80;

                        if (item.IsKhongNau)
                        {
                            dinhMucGam += 20;
                        }

                        float tongTruGam = dinhMucGam * item.SoLuong;

                        khoMonChinh.SoLuong -= (float)(tongTruGam / 1000.0);
                    }
                    else
                    {
                        Console.WriteLine($"[CẢNH BÁO TỒN KHO] Không tìm thấy nguyên liệu (NguyenLieuId = {spChinh.NguyenLieuId}) của món cháo trong kho Shop {shopId}!");
                    }
                }
                // rau
                if (item.RauCuNguyenLieuId != null && item.RauCuNguyenLieuId > 0)
                {
                    var khoRau = khoShop.FirstOrDefault(k => k.NguyenLieuId == item.RauCuNguyenLieuId);
                    if (khoRau != null) khoRau.SoLuong -= item.SoLuong;
                }

                // topping
                if (item.OrderDetailToppings != null)
                {
                    foreach (var top in item.OrderDetailToppings)
                    {
                        var khoTop = khoShop.FirstOrDefault(k => k.NguyenLieuId == top.ToppingSanPhamId);
                        if (khoTop != null) khoTop.SoLuong -= top.SoLuong;
                    }
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
