using backend.Data;
using backend.DTOs.Admin;
using backend.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ShopContext _context;
        public RoleController(ShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.ChucVu
                    .ToListAsync();
            return Ok(list);
        }

        [HttpPost]
        [Authorize("Giám đốc,Nhân sự")]
        public async Task<IActionResult> Create([FromForm] ChucVuDTO dto)
        {
            var role = new ChucVu
            {
                ChucVuName=dto.name, 
                ChucVuDescription=dto.description
            };

            _context.ChucVu.Add(role);
            await _context.SaveChangesAsync();
            return Ok( $"tạo chức vụ mới thành công {role}");
        }

        [HttpDelete]
        [Authorize("Giám đốc,Nhân sự")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.ChucVu.FindAsync(id);
            if(role == null)
            {
                return NotFound("k tìm thấy chức vụ cần xóa ");
            }
            _context.Remove(role);
            await _context.SaveChangesAsync();
            return Ok($"xóa thành công chức vụ mã {id}");
        }

        [HttpPut("role-update/{id}")]
        [Authorize("Giám đốc,Nhân sự")]
        public async Task<IActionResult> Update(int id,[FromBody] ChucVuDTO dto)
        {
            var role = await _context.ChucVu.FindAsync(id);
            if( role == null)
            {
                return NotFound("k tìm thấy chưucs vụ cần sửa");
            }

            if(!string.IsNullOrEmpty(dto.name)) role.ChucVuName= dto.name;
            if(!string.IsNullOrEmpty(dto.description)) role.ChucVuDescription= dto.description;

            await _context.SaveChangesAsync();
            return Ok($"cập nhật chức vụ thành công {role}");
        }

        [HttpGet("search-by-name")]
        public async Task<IActionResult> SearchByName([FromQuery] string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return await GetAll();
            }

            var result = await _context.ChucVu
                                .Where(s=>s.ChucVuName.Contains(key))
                                .ToListAsync();
            return Ok(result);
        }
    }
}
