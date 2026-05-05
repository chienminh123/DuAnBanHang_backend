using backend.Data;
using backend.DTOs.Admin;
using backend.Models.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {

        private readonly ShopContext _context;
        public ShiftController(ShopContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.CaLamViec.ToListAsync();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ShiftDTO request)
        {
            if (string.IsNullOrEmpty(request.TenCa))
            {
                return BadRequest("Tên ca làm việc không được để trống");
            }

            var shift = await _context.CaLamViec.FirstOrDefaultAsync(c => c.TenCa == request.TenCa.Trim());
            if (shift != null)
            {
                return BadRequest("Tên ca làm việc đã tồn tại");
            }

            var newShift = new CaLamViec
            {
                TenCa = request.TenCa,
                GioBatDau = request.GioBatDau,
                GioKetThuc = request.GioKetThuc
            };
            _context.CaLamViec.Add(newShift);
            await _context.SaveChangesAsync();
            return Ok("Tạo ca làm việc thành công");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var shift = await _context.CaLamViec.FindAsync(id);
            if (shift == null)
            {
                return NotFound("Không tồn tại ca làm việc này");
            }
            _context.CaLamViec.Remove(shift);
            await _context.SaveChangesAsync();
            return Ok("Xóa ca làm việc thành công");
        }

        [HttpPut("update-shift/{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ShiftDTO request)
        {
            var shift = await _context.CaLamViec.FirstOrDefaultAsync(c => c.Id == id);
            if (shift == null)
            {
                return NotFound("Không tồn tại ca làm việc này");
            }

            shift.TenCa = request.TenCa;
            shift.GioBatDau = request.GioBatDau;
            shift.GioKetThuc = request.GioKetThuc;
            await _context.SaveChangesAsync();
            return Ok("Cập nhật ca làm việc thành công");
        }


    }
}
