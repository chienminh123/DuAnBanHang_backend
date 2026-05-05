using backend.Data;
using backend.DTOs;
using backend.DTOs.Admin;

using backend.Models.Admin;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace backend.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ShopContext _context;
        private readonly TokenService _tokenService;
        private readonly IWebHostEnvironment _env;
        private readonly EmailService _emailService;

        public AuthController(ShopContext context, TokenService tokenService, IWebHostEnvironment env, EmailService emailService)
        {
            _context = context;
            _tokenService = tokenService;
            _env = env;
            _emailService = emailService;
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin, HR")]
        public async Task<IActionResult> Register_NoiBo([FromForm] RegisterAdminDTO request)
        {
            if (await _context.TaiKhoanNoiBo.AnyAsync(u => u.TenTaiKhoan == request.TenTaiKhoan)) 
            {
                return BadRequest("Tai Khoản này đã tồn tại!");
            }
            if (CheckPhone(request.Sdt) == false) return BadRequest("sdt k dúngd định dạng");
            string avatarPath = "";
            if (request.AvatarUpload != null)
            {
                var tenFile = Guid.NewGuid().ToString() + Path.GetExtension(request.AvatarUpload.FileName);
                var path = Path.Combine(_env.WebRootPath, "images", "avatar", tenFile);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await request.AvatarUpload.CopyToAsync(stream);
                }
                avatarPath = "/images/avatar/" + tenFile;
            }

            string PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.MatKhau);

            var user = new TaiKhoanNoiBo
            {
               TenTaiKhoan = request.TenTaiKhoan,
               MatKhau=PasswordHash,
               ChucVuId=request.ChucVuId,
               TenNhanVien=request.TenNhanVien,
               GioiTinh=request.GioiTinh,
               Sdt=request.Sdt,
               Email=request.Email,
               NgaySinh=request.NgaySinh,
               Avatar=avatarPath,
               NgayThamGia=DateTime.Now,
               CuaHangId=request.CuaHangId,
               IsActive=true
            };
            _context.TaiKhoanNoiBo.Add(user);
            await _context.SaveChangesAsync();

            try
            {
                await _emailService.SendWelcomeEmailAsync(
                    request.Email,
                    request.TenNhanVien,
                    request.TenTaiKhoan,
                    request.MatKhau // pass chưa mã hóa
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi gửi mail: " + ex.Message);
            }

            return Ok(new { Message = "Đăng kí thành công!",data =user });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login_NoiBo(LoginDTO request)
        {
            var user = await _context.TaiKhoanNoiBo
                                    .Include(u => u.ChucVu)
                                    .Include(u => u.CuaHang)
                                    .FirstOrDefaultAsync(u=>u.TenTaiKhoan==request.TenTaiKhoan);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.MatKhau, user.MatKhau))
            {
                return BadRequest("Tên tài khoản hoặc mật khẩu không đúng!");
            }
            string token = _tokenService.CreateToken(user.TenTaiKhoan,user.ChucVu.ChucVuName,user.TaiKhoanNoiBoId.ToString());
            return Ok(new {
                token=token ,
                Message = "Đăng nhập thành công!" ,
                tenCuaHang = user.CuaHang != null ? user.CuaHang.ShopName : "Cửa hàng chính",
                shopId = user.CuaHangId ?? 1
            });
        }

        private bool CheckPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return false;

            string pattern = @"^(0)(3|5|7|8|9)[0-9]{8}$";
            return Regex.IsMatch(phone, pattern);
        }
    }
}
