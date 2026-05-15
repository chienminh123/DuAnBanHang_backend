using backend.Data;
using backend.DTOs.Client;
using backend.Helpers;
using backend.Hubs;
using backend.Migrations;
using backend.Models.Admin;
using backend.Models.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace backend.Controllers.Client
{
    [Route("api/client/[controller]")]
    [ApiController]
    [Authorize] 
    public class OrderController : ControllerBase
    {
        private readonly ShopContext _context;
        private readonly IHubContext<OrderHub> _hubContext;

        public OrderController(ShopContext context,IHubContext<OrderHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userIdStr = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var orders = await _context.Order
                .Where(o => o.KhachHangId == userId)
                .OrderByDescending(o => o.NgayTao) 
                .Select(o => new {
                    o.Id,
                    o.MaDonHang,
                    o.NgayTao,
                    o.TongTienHang,
                    o.PhiGiaoHang,
                    o.ThanhTien,
                    o.TrangThaiDonHang,
                    o.PhuongThucThanhToan
                })
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("my-orders/{orderId}")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var userIdStr = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var order = await _context.Order
                .Include(o => o.CuaHang)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.RauCu)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.OrderDetailToppings)
                        .ThenInclude(odt => odt.SanPhamTopping)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.KhachHangId == userId);

            if (order == null) return NotFound("Không tìm thấy đơn hàng.");

            var spIds = order.OrderDetails.Select(od => od.SanPhamId).ToList();
            var sanPhams = await _context.sanPhams.Where(s => spIds.Contains(s.SanPhamId)).ToListAsync();

            var result = new
            {
                order.Id,
                order.MaDonHang,
                order.NgayTao,
                order.TrangThaiDonHang,
                order.TenNguoiNhan,
                order.SdtNguoiNhan,
                order.DiaChiGiaoHang,
                order.GhiChu,
                order.PhuongThucThanhToan,
                order.TongTienHang,
                order.PhiGiaoHang,
                order.ThanhTien,
                ShopName = order.CuaHang?.ShopName ?? "Cửa hàng trung tâm",
                Items = order.OrderDetails.Select(od => {
                    var spChinh = sanPhams.FirstOrDefault(s => s.SanPhamId == od.SanPhamId);
                    return new
                    {
                        TenMon = spChinh?.SanPhamName ?? "Món ăn",
                        HinhAnh = spChinh?.HinhAnh,
                        Size = spChinh?.Size ?? "M",
                        od.SoLuong,
                        od.DonGia,
                        od.ThanhTien,
                        od.IsKhongNau,
                        od.GhiChu,
                        RauCu = od.RauCu?.NguyenLieuName,
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


        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckOutDTO request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var userIdStr = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

                var cart = await _context.Cart
                    .Include(c => c.CartDetails)
                        .ThenInclude(cd => cd.Toppings)
                    .FirstOrDefaultAsync(c => c.KhachHangId == userId);

                if (cart == null || !cart.CartDetails.Any())
                {
                    return BadRequest("Giỏ hàng của bạn đang trống.");
                }

                var newOrder = new Order
                {
                    MaDonHang = "ORD" + DateTime.Now.Ticks.ToString(),
                    LoaiDonHang = "ONLINE",                            
                    ShopId = request.ShopId,                           
                    KhachHangId = userId,
                    NgayTao = DateTime.Now,
                    TrangThaiDonHang = request.PhuongThucThanhToan == "VNPAY" ? "CHO_THANH_TOAN" : "CHO_XAC_NHAN",
                    TenNguoiNhan = request.TenNguoiNhan,
                    SdtNguoiNhan = request.SdtNguoiNhan,
                    DiaChiGiaoHang = request.DiaChiGiaoHang,
                    PhuongThucThanhToan = request.PhuongThucThanhToan,
                    GhiChu = request.GhiChuDonHang,

                    TongTienHang = request.TongTienHang,
                    PhiGiaoHang = request.PhiGiaoHang,
                    TienGiamGia = request.TienGiamGia, 
                    DiemSuDung = request.DiemSuDung,
                    ThanhTien = request.TongTienHang + request.PhiGiaoHang
                };
                if (newOrder.ThanhTien < 0) newOrder.ThanhTien = 0;

                _context.Order.Add(newOrder);
                await _context.SaveChangesAsync(); 

                var sanPhams = await _context.sanPhams.ToListAsync();

                foreach (var cartItem in cart.CartDetails)
                {
                    var spChinh = sanPhams.FirstOrDefault(s => s.SanPhamId == cartItem.SanPhamId);
                    double giaMonChinh = (double)(spChinh?.GiaBan ?? 0);

                    var orderDetail = new OrderDetail
                    {
                        OrderId = newOrder.Id,
                        SanPhamId = cartItem.SanPhamId,
                        SoLuong = cartItem.SoLuong,
                        DonGia = giaMonChinh,
                        IsKhongNau = cartItem.IsKhongNau,
                        GhiChu = cartItem.GhiChu,
                        RauCuNguyenLieuId = cartItem.RauCuNguyenLieuId,
                        ThanhTien = giaMonChinh * cartItem.SoLuong 
                    };
                    _context.OrderDetail.Add(orderDetail);
                    await _context.SaveChangesAsync(); 

                    if (cartItem.Toppings != null && cartItem.Toppings.Any())
                    {
                        foreach (var t in cartItem.Toppings)
                        {
                            var spTopping = sanPhams.FirstOrDefault(s => s.SanPhamId == t.ToppingSanPhamId);
                            var orderTopping = new OrderDetailTopping
                            {
                                OrderDetailId = orderDetail.Id,
                                ToppingSanPhamId = t.ToppingSanPhamId,
                                SoLuong = t.SoLuong,
                                GiaTopping = (double)(spTopping?.GiaBan ?? 0)
                            };
                            _context.OrderDetailTopping.Add(orderTopping);
                        }
                    }
                }

                _context.CartDetailTopping.RemoveRange(cart.CartDetails.SelectMany(cd => cd.Toppings));
                _context.CartDetail.RemoveRange(cart.CartDetails);
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("NewWebOrderAlert", newOrder.ShopId,newOrder.Id);
                await transaction.CommitAsync();

                if (request.PhuongThucThanhToan == "VNPAY")
                {
                    string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
                    string vnp_Returnurl = "http://localhost:5173/vnpay-return"; 

                    string vnp_TmnCode = "I53LTI16";
                    string vnp_HashSecret = "7RK2E3FQ8YHTT09P0HC8ADMR3RMJ6D06";

                    var vnpay = new backend.Helpers.VnPayLibrary();

                    vnpay.AddRequestData("vnp_Version", "2.1.0");
                    vnpay.AddRequestData("vnp_Command", "pay");
                    vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);

                    long amount = (long)((request.TongTienHang + request.PhiGiaoHang) * 100);
                    vnpay.AddRequestData("vnp_Amount", amount.ToString());
                    vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                    vnpay.AddRequestData("vnp_CurrCode", "VND");
                    vnpay.AddRequestData("vnp_IpAddr", backend.Helpers.VnPayLibrary.GetIpAddress(HttpContext));
                    vnpay.AddRequestData("vnp_Locale", "vn");
                    vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang Chao Dinh Duong " + newOrder.Id);
                    vnpay.AddRequestData("vnp_OrderType", "other");
                    vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                    vnpay.AddRequestData("vnp_TxnRef", newOrder.Id.ToString() + "_" + DateTime.Now.Ticks.ToString());

                    string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

                    return Ok(new { message = "Đang chuyển hướng sang VNPAY...", orderId = newOrder.Id, paymentUrl = paymentUrl });
                }

        
                return Ok(new { message = "Đặt hàng thành công!", orderId = newOrder.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Lỗi hệ thống khi đặt hàng: " + ex.Message);
            }
        }

        [HttpPut("update-status/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromQuery] string status)
        {
            var order = await _context.Order.FindAsync(orderId);
            if (order == null) return NotFound("Không tìm thấy đơn hàng.");

            order.TrangThaiDonHang = status;
            await _context.SaveChangesAsync();

            if (status == "HOAN_THANH" || status == "PAID")
            {
                await XuLySauKhiBanXong(order.Id, order.ShopId);
            }
            return Ok(new { message = "Cập nhật trạng thái thành công!" });
        }

        [HttpGet("nearest")]
        public async Task<IActionResult> GetNearestShop(double lat, double lng)
        {
            var shops = await _context.cuaHangs
                .Where(s => s.Latitude != null && s.Longitude != null)
                .ToListAsync();

            if (!shops.Any()) return NotFound("Hệ thống chưa có cửa hàng nào được thiết lập tọa độ.");

            var nearestShop = shops
                .Select(s => new {
                    Shop = s,
                    DistanceKm = backend.Helpers.LocationHelper.CalculateDistance(
                        lat, lng,
                        s.Latitude.Value, s.Longitude.Value
                    ) / 1000.0
                })
                .OrderBy(x => x.DistanceKm) 
                .FirstOrDefault();

            if (nearestShop == null) return NotFound();

            return Ok(new
            {
                ShopId = nearestShop.Shop.ShopId,
                TenCuaHang = nearestShop.Shop.ShopName,
                DiaChi = nearestShop.Shop.ShopAddress,
                KhoangCachKm = Math.Round(nearestShop.DistanceKm, 1),
                ViDo = nearestShop.Shop.Latitude,
                KinhDo = nearestShop.Shop.Longitude
            });
        }

        [HttpGet("vnpay-return-update")]
        public async Task<IActionResult> UpdateVnPayStatus([FromQuery] string vnp_TxnRef, [FromQuery] string vnp_ResponseCode)
        {
            if (string.IsNullOrEmpty(vnp_TxnRef)) return BadRequest("Thiếu mã giao dịch.");

            var orderIdStr = vnp_TxnRef.Split('_')[0];

            if (int.TryParse(orderIdStr, out int orderId))
            {
                var order = await _context.Order.FindAsync(orderId);

                if (order != null && order.TrangThaiDonHang == "CHO_THANH_TOAN")
                {
                    if (vnp_ResponseCode == "00")
                    {
                        order.TrangThaiDonHang = "CHO_XAC_NHAN";
                        order.IsThanhToan = true;

                        await _context.SaveChangesAsync();
                        return Ok(new { message = "Cập nhật thành công!" });
                    }
                    else
                    {
                        order.TrangThaiDonHang = "DA_HUY"; 
                        order.GhiChu = order.GhiChu + " (Khách hủy thanh toán VNPAY)"; 

                        await _context.SaveChangesAsync();
                        return BadRequest("Thanh toán thất bại hoặc khách đã hủy giao dịch.");
                    }
                }
            }
            return BadRequest("Đơn hàng không tồn tại hoặc sai trạng thái.");
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
                    if (order.DiemSuDung > 0)
                    {
                        khach.TichDiem -= (int)order.DiemSuDung;
                        if (khach.TichDiem < 0) khach.TichDiem = 0;
                    }
                    int diemCong = (int)(order.ThanhTien / 10000); // 10k = 1 điểm
                    khach.TichDiem += diemCong;
                    order.DiemCongThem = diemCong;
                    await _context.SaveChangesAsync();
                }
            
            }

            var phieuXuatBan = new BienLai
            {
                HanhDong = "XUAT_BAN",
                CuaHangId = shopId,
                NgayThucHien = DateTime.Now,
                TrangThai = "HOAN_THANH"
            };
            _context.BienLai.Add(phieuXuatBan);
            await _context.SaveChangesAsync();

            var listChiTietXuat = new List<ChiTietBienLai>();

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
                        float luongTruKg = (float)(tongTruGam / 1000.0);

                        khoMonChinh.SoLuong -= luongTruKg;

                        listChiTietXuat.Add(new ChiTietBienLai
                        {
                            BienLaiId = phieuXuatBan.Id,
                            NguyenLieuId = spChinh.NguyenLieuId,
                            Soluong = luongTruKg,
                            GhiChu = "Xuất bán đơn " + order.MaDonHang
                        });
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
                    if (khoRau != null)
                    {
                        float tongTruKg = (float)(0.08 * item.SoLuong);

                        khoRau.SoLuong -= tongTruKg;
                        listChiTietXuat.Add(new ChiTietBienLai
                        {
                            BienLaiId = phieuXuatBan.Id,
                            NguyenLieuId = item.RauCuNguyenLieuId.Value,
                            Soluong = item.SoLuong,
                            GhiChu = "Rau củ đơn " + order.MaDonHang
                        });
                    }
                }

                // topping
                if (item.OrderDetailToppings != null)
                {
                    foreach (var top in item.OrderDetailToppings)
                    {
                        var khoTop = khoShop.FirstOrDefault(k => k.NguyenLieuId == top.ToppingSanPhamId);
                        if (khoTop != null)
                        {
                            khoTop.SoLuong -= top.SoLuong;
                            listChiTietXuat.Add(new ChiTietBienLai
                            {
                                BienLaiId = phieuXuatBan.Id,
                                NguyenLieuId = top.ToppingSanPhamId,
                                Soluong = top.SoLuong,
                                GhiChu = "Topping đơn " + order.MaDonHang
                            });
                        }
                    }
                }
            }
            if (listChiTietXuat.Any())
            {
                _context.ChiTietBienLai.AddRange(listChiTietXuat);
            }
            await _context.SaveChangesAsync();
        }
    }
}