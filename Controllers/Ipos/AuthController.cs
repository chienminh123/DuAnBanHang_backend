using backend.Data;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers.Ipos
{
    [Route("api/ipos/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        
        //public AuthController(ShopContext context , TokenService tokenService)
        //{private readonly ShopContext _context;
        //private readonly TokenService _tokenService;

        //    _context = context;
        //    _tokenService = tokenService;
        //}

        //[HttpPost("login")]
        //public async Task<IActionResult> IposLogin([FromBody] LoginDTO request)
        //{
        //    var user = await _context.TaiKhoanNoiBo
        //        .Include(u => u.CuaHang)
        //        .FirstOrDefaultAsync(u => u.TenTaiKhoan == request.TenTaiKhoan);

        //    if (user == null || !BCrypt.Net.BCrypt.Verify(request.MatKhau, user.MatKhau))
        //    {
        //        return BadRequest("Tên tài khoản hoặc mật khẩu không đúng!");
        //    }

        //    if (!user.IsActive)
        //    {
        //        return BadRequest("Tài khoản này đã bị khóa!");
        //    }

        //    // 4. TẠO TOKEN (Bạn copy nguyên dòng _tokenService.CreateToken(...) từ file cũ sang đây)
        //    // string tokenString = _tokenService.CreateToken(...);

        //    // 5. TRẢ VỀ JSON ĐÃ "ĐÓNG GÓI" DÀNH RIÊNG CHO ANDROID
        //    return Ok(new
        //    {
        //        message = "Đăng nhập thành công",
        //        // token = tokenString, 
        //        tenNhanVien = user.TenNhanVien,
        //        // Nếu CuaHangId null (nhân viên tổng bộ), gán mặc định là "Cửa hàng chính"
        //        tenCuaHang = user.CuaHang != null ? user.CuaHang.TenCuaHang : "Cửa hàng chính"
        //    });
        //}
    }
}

