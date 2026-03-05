using backend.Data;
using backend.DTOs.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace backend.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ShopContext _context;
        private readonly IWebHostEnvironment _env;

        public EmployeeController (ShopContext context , IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.TaiKhoanNoiBo
                    .Include(s => s.ChucVu )
                    .Include(s => s.CuaHang )
                    .ToListAsync();
            return Ok(list);
        }

        [HttpDelete("id")]
        //[Authorize(Roles = "Giám đốc, Nhân sự")]
        public async Task<IActionResult> Delete(int id)
        {
            var nv = await _context.TaiKhoanNoiBo.FindAsync(id);
            if (nv == null)
            {
                return NotFound("Không tìm thấy nhân viên !");
            }
            if (!string.IsNullOrEmpty(nv.Avatar))
            {
                var oldPath = Path.Combine(_env.WebRootPath, nv.Avatar.TrimStart('/'));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }
            _context.TaiKhoanNoiBo.Remove(nv);
            await _context.SaveChangesAsync();
            return Ok($"Đã xóa nhân viên mã {id}");
        }

        [HttpPut("admin-update/{id}")]
        //[Authorize(Roles = "Giám đốc, Nhân sự")]
        public async Task<IActionResult> UpdateByAdmin(int id, [FromForm] NhanVienDTO request, [FromForm] int? newChucVuId, [FromForm] int? newCuaHangId, [FromForm] string? newTenDangNhap)
        {
            var nv = await _context.TaiKhoanNoiBo.FindAsync(id);
            if (nv == null) return NotFound();

            if (!string.IsNullOrEmpty(nv.Avatar)) nv.TenTaiKhoan = newTenDangNhap;
            if (!string.IsNullOrEmpty(request.name)) nv.TenNhanVien = request.name;
            if (!string.IsNullOrEmpty(request.sdt)&&CheckPhone(request.sdt)==true) nv.Sdt = request.sdt;
            if (!string.IsNullOrEmpty(request.email)) nv.Email = request.email;
            if (request.NgaySinh != null) nv.NgaySinh = request.NgaySinh;
            if (request.GioiTinh != null) nv.GioiTinh = request.GioiTinh;
            if (request.AvatarUpload != null)
            {

                if (!string.IsNullOrEmpty(nv.Avatar))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, nv.Avatar.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }
                var tenFile = Guid.NewGuid().ToString() + Path.GetExtension(request.AvatarUpload.FileName);
                var path = Path.Combine(_env.WebRootPath, "images", "avatar", tenFile);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await request.AvatarUpload.CopyToAsync(stream);
                }
                nv.Avatar = "/images/avatar/" + tenFile;
            }

            if (newChucVuId != null) nv.ChucVuId = newChucVuId.Value;
            if (newCuaHangId != null) nv.CuaHangId = newCuaHangId.Value;

            await _context.SaveChangesAsync();
            return Ok("Admin đã cập nhật nhân viên thành công!");
        }

        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateSelf([FromForm] NhanVienDTO request)
        {
            // Lấy ID người đang đăng nhập từ Token
            var userIdStr = User.FindFirst("Id")?.Value;
            if (userIdStr == null) return Unauthorized("Không tìm thấy thông tin đăng nhập");

            int id = int.Parse(userIdStr);
            var nv = await _context.TaiKhoanNoiBo.FindAsync(id);
            if (nv == null) return NotFound();

            if (!string.IsNullOrEmpty(request.name)) nv.TenNhanVien = request.name;
            if (!string.IsNullOrEmpty(request.sdt) && CheckPhone(request.sdt) == true) nv.Sdt = request.sdt;
            if (!string.IsNullOrEmpty(request.email)) nv.Email = request.email;
            if (request.NgaySinh != null) nv.NgaySinh = request.NgaySinh;
            if (request.GioiTinh != null) nv.GioiTinh = request.GioiTinh;

            await _context.SaveChangesAsync();
            return Ok("Cập nhật thông tin cá nhân thành công!");
        }

        [HttpPatch("update-avatar")]
        [Authorize]
        public async Task<IActionResult> UpdateAvatar([FromForm] IFormFile avatar)
        {
            var userIdStr = User.FindFirst("Id")?.Value;
            if (userIdStr == null) return Unauthorized("Không tìm thấy thông tin đăng nhập");

            int id = int.Parse(userIdStr);
            var nv = await _context.TaiKhoanNoiBo.FindAsync(id);
            if (nv == null) return NotFound();

            if (avatar != null && avatar.Length > 0)
            {

                if (!string.IsNullOrEmpty(nv.Avatar))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, nv.Avatar.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }              
                var tenFile = Guid.NewGuid().ToString() + Path.GetExtension(avatar.FileName);
                var path = Path.Combine(_env.WebRootPath, "images", "avatar", tenFile);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await avatar.CopyToAsync(stream);
                }
                nv.Avatar = "/images/avatar/" + tenFile;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Cập nhật ảnh đại diện thành công!", url = nv.Avatar });
            }
            return BadRequest("Vui lòng chọn file ảnh hợp lệ");
        }

        [HttpPatch("update-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromForm] string matKhauMoi)
        {
            var userIdStr = User.FindFirst("Id")?.Value;
            if (userIdStr == null) return Unauthorized();
            int id = int.Parse(userIdStr);
            var nv = await _context.TaiKhoanNoiBo.FindAsync(id);
            if (nv == null) return NotFound();

            if (string.IsNullOrEmpty(matKhauMoi) || matKhauMoi.Length < 6)
                return BadRequest("Mật khẩu mới phải có ít nhất 6 ký tự");
            else if (matKhauMoi == nv.MatKhau)
                return BadRequest("Mật khẩu mới phải khác mật khẩu cũ");
            // Hash mật khẩu mới
            nv.MatKhau = BCrypt.Net.BCrypt.HashPassword(matKhauMoi);

            await _context.SaveChangesAsync();
            return Ok("Đổi mật khẩu thành công!");
        }

        [HttpGet("search-by-name")]
        public async Task<IActionResult> SearchByName([FromQuery] string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return await GetAll();
            }

            var result = await _context.TaiKhoanNoiBo
                .Include(s => s.CuaHang)
                .Include(s=>s.ChucVu)
                .Where(s => s.TenNhanVien.Contains(key))
                .ToListAsync();

            if (result.Count == 0)
            {
                return NotFound("Không tìm thấy nhân viên nào khớp với từ khóa.");
            }
            return Ok(result);
        }

        [HttpGet("search-by-store")]
        public async Task<IActionResult> SearchByStore ([FromQuery] string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return await GetAll();
            }

            var result = await _context.TaiKhoanNoiBo
                .Include(s => s.CuaHang)
                .Include(s => s.ChucVu)
                .Where(s => s.CuaHang.ShopName.Contains(key))
                .ToListAsync();

            if (result.Count == 0)
            {
                return NotFound("Không tìm thấy NV nào thuộc cửa hàng khớp với từ khóa.");
            }
            return Ok(result);
        }

        [HttpGet("search-by-id")]
        public async Task<IActionResult> SearchById ([FromQuery] int  id)
        {
            var result = await _context.TaiKhoanNoiBo.FindAsync(id);

            if (result == null)
            {
                return NotFound($"Không tìm thấy nhân viên  nào có mã NVlà {id}.");
            }
            return Ok(result);
        }

        [HttpGet("search-by-role")]
        public async Task<IActionResult> SearchByRole([FromQuery] string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return await GetAll();
            }

            var result = await _context.TaiKhoanNoiBo
                .Include(s => s.CuaHang)
                .Include(s => s.ChucVu)
                .Where(s => s.ChucVu.ChucVuName.Contains(key))
                .ToListAsync();

            if (result.Count == 0)
            {
                return NotFound("Không tìm thấy NV nào có chức vụ khớp với từ khóa.");
            }
            return Ok(result);
        }
        private bool CheckPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return false;

            string pattern = @"^(0)(3|5|7|8|9)[0-9]{8}$";
            return Regex.IsMatch(phone, pattern);
        }
    }
}
