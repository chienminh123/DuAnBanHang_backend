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
        public async Task<IActionResult> SendOtp([FromBody] RegisterClientDTO request)
        {
            // Validate email chưa tồn tại
            if (await _context.taiKhoanKhachHang.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Tài khoản với email này đã tồn tại!");
            }

            
            if (!System.Text.RegularExpressions.Regex.IsMatch(request.sdt, @"^\d{10,11}$"))
            {
                return BadRequest("Số điện thoại không hợp lệ!");
            }

            Random generator = new Random();
            string otpCode = generator.Next(0, 1000000).ToString("D6");
            _memoryCache.Set("OTP_" + request.Email, otpCode, TimeSpan.FromMinutes(5));
            string subject = "Mã xác thực đăng ký NMCcuteShop";
            string body = $"Mã OTP của bạn là: {otpCode}. Mã có hiệu lực trong 5 phút.";
            try
            {
                await _emailService.SendEmailAsync(request.Email, subject, body);
                return Ok($"Đã gửi mã OTP về email {request.Email}. Vui lòng kiểm tra hộp thư!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi gửi mail: " + ex.Message);
            }
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Email không hợp lệ.");
            string otpCode = new Random().Next(100000, 999999).ToString();

            // Lưu vào Cache (đè lên mã cũ)
            _memoryCache.Set("OTP_" + email, otpCode, TimeSpan.FromMinutes(5));

            await _emailService.SendEmailAsync(email, "Gửi lại mã OTP xác nhận", $"Mã OTP mới của bạn là: <b>{otpCode}</b>");

            return Ok(new { Message = "Mã OTP mới đã được gửi vào Email của bạn." });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register_client(RegisterClientDTO request)
        {
            if (!_memoryCache.TryGetValue("OTP_" + request.Email, out string savedOtp))
            {
                return BadRequest("Mã OTP đã hết hạn hoặc chưa được gửi.");
            }
            if (savedOtp != request.OtpCode)
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
