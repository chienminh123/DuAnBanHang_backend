using backend.Data;
using backend.DTOs.Client;
using backend.Models.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.Client
{
    [Route("api/client/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ShopContext _context;

        public CartController(ShopContext context)
        {
            _context = context;
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCartCount()
        {
            var userIdStr = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            int totalCount = await _context.CartDetail
                                    .Where(cd => cd.Cart.KhachHangId == userId)
                                    .SumAsync(c => c.SoLuong);
                                   

            return Ok(totalCount);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] CartDTO request)
        {
            var userIdStr = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized("Token không hợp lệ.");
            }

            var cart = await _context.Cart
                .Include(c => c.CartDetails)
                    .ThenInclude(cd => cd.Toppings)
                .FirstOrDefaultAsync(c => c.KhachHangId == userId);

            if (cart == null)
            {
                cart = new Cart { KhachHangId = userId };
                _context.Cart.Add(cart);
                await _context.SaveChangesAsync(); 
            }

            CartDetail? existingItem = null;

            foreach (var item in cart.CartDetails)
            {
                if (item.SanPhamId == request.SanPhamId &&
                    item.IsKhongNau == request.IsKhongNau &&
                    item.RauCuNguyenLieuId == request.RauCuNguyenLieuId && 
                    item.GhiChu == request.GhiChu)
                {
                    var existingToppings = item.Toppings.OrderBy(t => t.ToppingSanPhamId).ToList();
                    var requestToppings = request.Toppings.OrderBy(t => t.ToppingSanPhamId).ToList();

                    bool isToppingMatch = true;

                    if (existingToppings.Count != requestToppings.Count)
                    {
                        isToppingMatch = false;
                    }
                    else
                    {
                        for (int i = 0; i < existingToppings.Count; i++)
                        {
                            if (existingToppings[i].ToppingSanPhamId != requestToppings[i].ToppingSanPhamId ||
                                existingToppings[i].SoLuong != requestToppings[i].SoLuong)
                            {
                                isToppingMatch = false;
                                break;
                            }
                        }
                    }

                    if (isToppingMatch)
                    {
                        existingItem = item;
                        break;
                    }
                }
            }

            if (existingItem != null)
            {
                existingItem.SoLuong += request.SoLuong;
            }
            else
            {
                var newItem = new CartDetail
                {
                    CartId = cart.Id,
                    SanPhamId = request.SanPhamId,
                    SoLuong = request.SoLuong,
                    IsKhongNau = request.IsKhongNau,
                    RauCuNguyenLieuId = request.RauCuNguyenLieuId, 
                    GhiChu = request.GhiChu,
                    Toppings = request.Toppings.Select(t => new CartDetailTopping
                    {
                        ToppingSanPhamId = t.ToppingSanPhamId, 
                        SoLuong = t.SoLuong
                    }).ToList()
                };
                _context.CartDetail.Add(newItem);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã thêm vào giỏ hàng thành công!" });
        }

        [HttpGet("my-cart")]
        public async Task<IActionResult> GetMyCart()
        {
            var userIdStr = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var cart = await _context.Cart
                .Include(c => c.CartDetails)
                    .ThenInclude(cd => cd.RauCu) 
                .Include(c => c.CartDetails)
                    .ThenInclude(cd => cd.Toppings)
                        .ThenInclude(t => t.SanPhamTopping)
                .FirstOrDefaultAsync(c => c.KhachHangId == userId);

            if (cart == null || !cart.CartDetails.Any())
            {
                return Ok(new List<object>()); 
            }

            var sanPhams = await _context.sanPhams.Include(s => s.TheLoai).ToListAsync();

            var result = cart.CartDetails.Select(cd => {
                var spChinh = sanPhams.FirstOrDefault(s => s.SanPhamId == cd.SanPhamId);

                return new
                {
                    CartDetailId = cd.Id,
                    SanPhamId = cd.SanPhamId,
                    TenMon = spChinh?.SanPhamName ?? "Món ăn",
                    HinhAnh = spChinh?.HinhAnh,
                    GiaBan = spChinh?.GiaBan ?? 0,
                    Size = spChinh?.Size ?? "S",
                    SoLuong = cd.SoLuong,
                    IsKhongNau = cd.IsKhongNau,
                    GhiChu = cd.GhiChu,
                    RauCu = cd.RauCu != null ? cd.RauCu.NguyenLieuName : null,
                    Toppings = cd.Toppings.Select(t => new {
                        TenTopping = t.SanPhamTopping?.SanPhamName,
                        GiaTopping = t.SanPhamTopping?.GiaBan ?? 0,
                        SoLuong = t.SoLuong
                    }).ToList()
                };
            }).ToList();

            return Ok(result);
        }

        [HttpDelete("remove/{cartDetailId}")]
        public async Task<IActionResult> RemoveFromCart(int cartDetailId)
        {
            var userIdStr = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var cartDetail = await _context.CartDetail
                .Include(cd => cd.Cart)
                .Include(cd => cd.Toppings) 
                .FirstOrDefaultAsync(cd => cd.Id == cartDetailId && cd.Cart.KhachHangId == userId);

            if (cartDetail == null) return NotFound("Không tìm thấy món trong giỏ.");

            _context.CartDetailTopping.RemoveRange(cartDetail.Toppings); 
            _context.CartDetail.Remove(cartDetail); 
            await _context.SaveChangesAsync();

            return Ok("Đã xóa khỏi giỏ hàng.");
        }

        [HttpPut("update-qty/{cartDetailId}")]
        public async Task<IActionResult> UpdateQuantity(int cartDetailId, [FromBody] int soLuongMoi)
        {
            if (soLuongMoi < 1) return BadRequest("Số lượng không hợp lệ.");

            var cartDetail = await _context.CartDetail.FindAsync(cartDetailId);
            if (cartDetail == null) return NotFound("Không tìm thấy món.");

            cartDetail.SoLuong = soLuongMoi;
            await _context.SaveChangesAsync();

            return Ok("Cập nhật thành công.");
        }
    }
}
     
