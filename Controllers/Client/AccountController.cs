using backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.Client
{
    [Route("api/client/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly ShopContext _context;

        public AccountController(ShopContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var idClaim = User?.Claims?.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (string.IsNullOrWhiteSpace(idClaim) || !int.TryParse(idClaim, out var id))
            {
                return Unauthorized(new { message = "Token không hợp lệ." });
            }

            var user = await _context.taiKhoanKhachHang
                .Include(u => u.ChucVu)
                .FirstOrDefaultAsync(u => u.KhachHangId == id);

            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy khách hàng." });
            }

            return Ok(new
            {
                khachHangId = user.KhachHangId,
                tenKhachHang = user.TenKhachHang,
                sdt = user.Sdt,
                email = user.Email,
                tichDiem = user.TichDiem,
                role = user.ChucVu?.ChucVuName ?? ""
            });
        }
    }
}

