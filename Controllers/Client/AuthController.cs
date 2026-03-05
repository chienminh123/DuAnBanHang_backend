using backend.Data;
using backend.DTOs;
using backend.DTOs.Client;
using backend.Models.Client;
using backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace backend.Controllers.Client
{
    [Route("api/client/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ShopContext _context;
        private readonly TokenService _tokenService;
        private readonly IMemoryCache _memoryCache;
        private readonly EmailService _emailService;

        public AuthController(ShopContext context, TokenService tokenService,IMemoryCache memoryCache, EmailService emailService)
        {
            _context = context;
            _tokenService = tokenService;
            _memoryCache = memoryCache;
            _emailService = emailService;
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] string email)
        {
            if (await _context.taiKhoanKhachHang.AnyAsync(u => u.Email == email))
            {
                return BadRequest("Tai Khoản này đã tồn tại!");
            }
            Random generator = new Random();
            string otpCode = generator.Next(0, 1000000).ToString("D6");
            _memoryCache.Set("OTP_" + email, otpCode, TimeSpan.FromMinutes(5));
            string subject = "Mã xác thực đăng ký NMCcuteShop";
            string body = $"Mã OTP của bạn là: {otpCode}. Mã có hiệu lực trong 5 phút.";
            try
            {
                await _emailService.SendEmailAsync(email, subject, body);
                return Ok($"Đã gửi mã OTP về email {email}. Vui lòng kiểm tra hộp thư!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi gửi mail: " + ex.Message);
            }
        }
        [HttpPost("regsiter")]
        public async Task<IActionResult> Regsiter_client(RegisterClientDTO request,string otpCode)
        {
            if (!_memoryCache.TryGetValue("OTP_" + request.Email, out string savedOtp))
            {
                return BadRequest("Mã OTP đã hết hạn hoặc chưa được gửi.");
            }
            if (savedOtp != otpCode)
            {
                return BadRequest("Mã OTP không chính xác.");
            }
            string PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.MatKhau);
            var user = new TaiKhoanKhachHang
            {
                TenKhachHang=request.HoTen,
                Sdt=request.sdt,
                Email=request.Email,
                MatKhau=PasswordHash,
                ChucVuId=7,
                NgaySinh=request.NgaySinh,
                NgayThamGia=DateTime.Now,
                TichDiem=0
            };
            _context.taiKhoanKhachHang.Add(user);
            await _context.SaveChangesAsync();
            _memoryCache.Remove("OTP_" + request.Email);
            return Ok(new { Message = "Đăng kí thành công!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login_client(LoginDTO request)
        {
            var user = await _context.taiKhoanKhachHang
                                    .Include(u => u.ChucVu)
                                    .FirstOrDefaultAsync(u => u.Sdt == request.TenTaiKhoan|| u.Email==request.TenTaiKhoan);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.MatKhau, user.MatKhau))
            {
                return BadRequest("Tên tài khoản hoặc mật khẩu không đúng!");
            }
            string token = _tokenService.CreateToken(user.Sdt, user.ChucVu.ChucVuName, user.KhachHangId.ToString());
            return Ok(new {token=token ,Message = "Đăng nhập thành công!" });
        }

    }
}
