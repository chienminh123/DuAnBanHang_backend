using backend.Data;
using backend.DTOs.Admin;
using backend.Models.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly ShopContext _context;

        public ScheduleController(ShopContext context)
        {
            _context = context;
        }
        [HttpGet("get_schedule")]
        public async Task<IActionResult> GetSchedule([FromQuery] int? shopId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var query = _context.LichLamViec.Where(l => l.NgayLamViec.Date >= startDate.Date && l.NgayLamViec.Date <= endDate.Date);

            if (shopId.HasValue && shopId.Value > 0)
            {
                query = query.Where(l => l.ShopId == shopId.Value);
            }

            var list = await query.ToListAsync();
            return Ok(list);
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveSchedule([FromBody] ScheduleDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var oldShifts = await _context.LichLamViec.Where(l =>
                l.TaiKhoanNoiBoId == request.TaiKhoanNoiBoId &&
                l.NgayLamViec.Date == request.NgayLamViec.Date).ToListAsync();

            if (oldShifts.Any())
            {
                _context.LichLamViec.RemoveRange(oldShifts);
            }

            var validShifts = request.CaLamViecIds.Where(id => id > 0).Distinct().ToList();

            foreach (var shiftId in validShifts)
            {
                _context.LichLamViec.Add(new LichLamViec
                {
                    TaiKhoanNoiBoId = request.TaiKhoanNoiBoId,
                    CaLamViecId = shiftId,
                    ShopId = request.ShopId,
                    NgayLamViec = request.NgayLamViec
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Lưu lịch thành công" });
        }
    }
}