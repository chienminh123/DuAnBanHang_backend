using backend.Data;
using backend.DTOs;
using backend.DTOs.Ipos;
using backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.Ipos
{
    [Route("api/ipos/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ShopContext _context;
        private readonly TokenService _tokenService;
        public AuthController(ShopContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }


        [HttpGet("check-phone")]
        public async Task<IActionResult> CheckPhone([FromQuery] string phone)
        {
            var khach = await _context.taiKhoanKhachHang
                .FirstOrDefaultAsync(k => k.Sdt == phone);

            if (khach == null)
                return NotFound();

            return Ok(new
            {
                id = khach.KhachHangId,
                tenKhachHang = khach.TenKhachHang ?? "Khách hàng"
            });
        }

        [HttpPost("create-fast-customer")]
        public async Task<IActionResult> CreateFastCustomer([FromBody] CreateFastCustomerDTO dto)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(dto.Phone, @"^(0)(3|5|7|8|9)[0-9]{8}$"))
            {
                return BadRequest("Số điện thoại không đúng định dạng!");
            }

            if (await _context.taiKhoanKhachHang.AnyAsync(k => k.Sdt == dto.Phone))
            {
                return BadRequest("Số điện thoại này đã được đăng ký!");
            }

            string defaultPasswordHash = BCrypt.Net.BCrypt.HashPassword("123456");

            var newKhach = new backend.Models.Client.TaiKhoanKhachHang
            {
                TenKhachHang = dto.Name,
                Sdt = dto.Phone,
                Email = dto.Phone + "@khachhang.com",
                MatKhau = defaultPasswordHash,
                ChucVuId = 7, 
                NgayThamGia = DateTime.Now,
                NgaySinh = DateTime.Now, 
                TichDiem = 0
            };

            _context.taiKhoanKhachHang.Add(newKhach);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = newKhach.KhachHangId,
                tenKhachHang = newKhach.TenKhachHang
            });
        }


    }
}


