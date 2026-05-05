using backend.Data;
using backend.Models.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.Ipos
{
    [Route("api/ipos/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly ShopContext _context;

        public MenuController(ShopContext context)
        {
            _context = context;
        }

        // 1. Lấy danh sách món theo mã CỬA HÀNG
        [HttpGet("manage-list/{shopId}")]
        public async Task<IActionResult> GetManageList(int shopId)
        {
            // Lấy tất cả món ăn ĐANG MỞ trên toàn hệ thống 
            var allProducts = await _context.sanPhams.Where(p => p.IsActive).ToListAsync();
            // Lấy cài đặt riêng của cửa hàng đang đăng nhập
            var shopSettings = await _context.MenuCuaHang.Where(m => m.ShopId == shopId).ToListAsync();

            var result = allProducts.Select(p => {
                // Tìm xem cửa hàng này đã từng gạt nút tắt/bật món này chưa
                var setting = shopSettings.FirstOrDefault(s => s.SanPhamId == p.SanPhamId);

                return new
                {
                    SanPhamId = p.SanPhamId,
                    SanPhamName = p.SanPhamName,
                    Size = p.Size,
                    GiaBan = p.GiaBan,
                    IsActive = setting != null ? setting.IsActive : true,
                    TheLoaiId = p.TheLoaiId
                };
            });

            return Ok(result);
        }

        //  Nhận lệnh gạt công tắc từ App Android (Theo Cửa hàng + Món ăn)
        [HttpPut("toggle-status/{shopId}/{sanPhamId}")]
        public async Task<IActionResult> ToggleStatus(int shopId, int sanPhamId)
        {
            var setting = await _context.MenuCuaHang
                .FirstOrDefaultAsync(m => m.ShopId == shopId && m.SanPhamId == sanPhamId);

            bool newStatus;

            if (setting == null)
            {
                setting = new MenuCuaHang
                {
                    ShopId = shopId,
                    SanPhamId = sanPhamId,
                    IsActive = false
                };
                _context.MenuCuaHang.Add(setting);
                newStatus = false;
            }
            else
            {
                setting.IsActive = !setting.IsActive;
                newStatus = setting.IsActive;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thành công!", isActive = newStatus });
        }

        // API Lấy danh mục dành riêng cho máy POS
        [HttpGet("get-categories")]
        public async Task<IActionResult> GetCategoriesForPOS()
        {
            // Chỉ lấy các thể loại: 2 (Cháo), 6 (Đồ chơi), 7 (Thương mại), 8 (Topping), 9 (Sữa hạt)
            int[] allowedIds = { 2, 6, 7, 8, 9 };

            var categories = await _context.theLoais
                .Where(t => allowedIds.Contains(t.TheLoaiId))
                .Select(t => new {
                    TheLoaiId = t.TheLoaiId,
                    TheLoaiName = t.TheLoaiName.Trim()
                })
                .ToListAsync();

            return Ok(categories);
        }

        // API Đổ dữ liệu ra màn hình Bán Hàng (POS) của Cửa Hàng
        [HttpGet("get-pos-menu/{shopId}")]
        public async Task<IActionResult> GetPosMenu(int shopId)
        {
            // Tìm các món BỊ TẮT ở cửa hàng này
            var disabledProducts = await _context.MenuCuaHang
                .Where(m => m.ShopId == shopId && m.IsActive == false)
                .Select(m => m.SanPhamId)
                .ToListAsync();

            // Lấy các món ĐANG BẬT trên hệ thống, và KHÔNG NẰM trong danh sách bị tắt ở trên
            var products = await _context.sanPhams
                .Where(p => p.IsActive == true && !disabledProducts.Contains(p.SanPhamId))
                .Select(p => new {
                    sanPhamId = p.SanPhamId,
                    sanPhamName = p.SanPhamName,
                    theLoaiId = p.TheLoaiId,
                    size = p.Size,
                    giaBan = p.GiaBan,
                    hinhAnh = p.HinhAnh
                })
                .ToListAsync();

            return Ok(products);
        }
        // API Lấy Rau Củ và Topping CỦA RIÊNG CỬA HÀNG ĐÓ
        [HttpGet("get-pos-extras/{shopId}")]
        public async Task<IActionResult> GetPosExtras(int shopId)
        {
            //  LẤY TOPPING (Lọc các món bị tắt ở MenuCuaHang)
            var disabledToppings = await _context.MenuCuaHang
                .Where(m => m.ShopId == shopId && m.IsActive == false)
                .Select(m => m.SanPhamId)
                .ToListAsync();

            var toppings = await _context.sanPhams
                .Include(s => s.TheLoai)
                .Where(s => s.IsActive == true &&
                            s.TheLoai != null && s.TheLoai.TheLoaiName.ToLower().Contains("topping") &&
                            !disabledToppings.Contains(s.SanPhamId))
                .Select(s => new {
                    id = s.SanPhamId,
                    name = s.SanPhamName,
                    price = s.GiaBan
                })
                .ToListAsync();

            var veggies = await _context.tonKhos
                .Include(t => t.NguyenLieu)
                    .ThenInclude(n => n.TheLoai)
                .Where(t => t.ShopId == shopId &&
                            t.SoLuong > 0 &&
                            t.NguyenLieu != null && t.NguyenLieu.IsActive == true &&
                            t.NguyenLieu.TheLoai != null && t.NguyenLieu.TheLoai.TheLoaiName.ToLower().Contains("rau củ"))
                .Select(t => new {
                    id = t.NguyenLieuId,
                    name = t.NguyenLieu.NguyenLieuName
                })
                .ToListAsync();

            return Ok(new { veggies, toppings });
        }

    }


}