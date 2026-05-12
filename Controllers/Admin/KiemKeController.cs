using backend.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.Admin
{
    [Route("api/Admin/[controller]")]
    [ApiController]
    public class KiemKeController : ControllerBase
    {
        private readonly ShopContext _context;

        public KiemKeController(ShopContext context)
        {
            _context = context;
        }

        [HttpGet("danh_sach/{shopId}")]
        public async Task<IActionResult> GetDanhSachKiemKe(int shopId)
        {
            var list = await _context.KiemKe
                .Where(k => k.ShopId == shopId)
                .OrderByDescending(k => k.NgayThucHien)
                .Select(k => new {
                    k.KiemKeId,
                    k.NgayThucHien,
                    TongSoMon = _context.ChiTietKiemKe.Count(ct => ct.KiemKeId == k.KiemKeId),
                    TenCuaHang = k.CuaHang.ShopName
                })
                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("chi_tiet/{kiemKeId}")]
        public async Task<IActionResult> GetChiTietKiemKe(int kiemKeId)
        {
            var detail = await _context.KiemKe
                .Include(k => k.CuaHang)
                .Where(k => k.KiemKeId == kiemKeId)
                .Select(k => new {
                    k.KiemKeId,
                    k.NgayThucHien,
                    TenCuaHang = k.CuaHang.ShopName,
                    DanhSachChiTiet = _context.ChiTietKiemKe
                        .Include(ct => ct.NguyenLieu)
                        .Where(ct => ct.KiemKeId == kiemKeId)
                        .Select(ct => new {
                            ct.Id,
                            TenNguyenLieu = ct.NguyenLieu.NguyenLieuName,
                            DonVi = ct.NguyenLieu.DonVi, 
                            ct.TonHeThong,
                            ct.TonThucTe,
                            ChenhLech = ct.TonThucTe - ct.TonHeThong,
                            GhiChu = ct.Note
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            if (detail == null) return NotFound();
            return Ok(detail);
        }
    } 
}
