using backend.Data;
using backend.DTOs.Ipos;
using backend.Models.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.Ipos
{
    [Route("api/ipos/[controller]")]
    [ApiController]
    public class ChotCaController : ControllerBase
    {
        private readonly ShopContext _context;

        public ChotCaController(ShopContext context)
        {
            _context = context;
        }

        [HttpGet("check-ca/{shopId}")]
        public async Task<IActionResult> CheckCa([FromBody]MoCaDTO dto)
        {
            var caDangMo = await _context.ChotCa
                                    .FirstOrDefaultAsync(cc => cc.ShopId == dto.ShopId && cc.TrangThai == "DANG_MO");
            if (caDangMo==null)
            {
                return Ok(new { isDaMo = false });
            }
            return Ok(new { isDaMo = true, ca = caDangMo });
        }

        [HttpGet("mo-ca")]
        public async Task<IActionResult> MoCa([FromBody]MoCaDTO dto)
        {
            bool dangCoCaMo = await _context.ChotCa.AnyAsync(c => c.ShopId == dto.ShopId && c.TrangThai == "DANG_MO");
            if (dangCoCaMo) return BadRequest("Cửa hàng này đang có ca làm việc chưa đóng!");

            var caMoi = new ChotCa
            {
                ShopId = dto.ShopId,
                ThoiGianMo = DateTime.Now,
                TienDauCa = dto.TienDauCa ?? 0,
                TongThuTienMat = 0,
                TongThuVNPAY = 0,
                TrangThai = "DANG_MO"
            };

            _context.ChotCa.Add(caMoi);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Mở ca thành công!", caId = caMoi.Id });
        }

        [HttpPost("dong-ca/{caId}")]
        public async Task<IActionResult> DongCa(int caId, [FromBody] DongCaDTO dto)
        {
            var ca = await _context.ChotCa.FindAsync(caId);
            if (ca == null || ca.TrangThai == "DA_CHOT") return BadRequest("Ca làm việc không hợp lệ hoặc đã chốt.");

            ca.ThoiGianDong = DateTime.Now;
            ca.TienMatThucTe = dto.TienMatThucTe;
            ca.VnpayThucTe = dto.VnpayThucTe;
            ca.GhiChu = dto.GhiChu;
            ca.TrangThai = "DA_CHOT";
            // Tính tổng thu từ đơn hàng trong ca
            var donHangs = await _context.Order
                .Where(o => o.ShopId == ca.ShopId
                         && o.NgayTao >= ca.ThoiGianMo
                         && o.NgayTao <= ca.ThoiGianDong
                         && (o.TrangThaiDonHang == "DA_THANH_TOAN" || o.TrangThaiDonHang == "HOAN_THANH" || o.TrangThaiDonHang == "PAID" || o.IsThanhToan == true))
                .ToListAsync();

            ca.TongThuTienMat = donHangs.Where(o => o.PhuongThucThanhToan == "TIEN_MAT").Sum(o => o.ThanhTien);
            ca.TongThuVNPAY = donHangs.Where(o => o.PhuongThucThanhToan == "VNPAY").Sum(o => o.ThanhTien);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Đóng ca thành công!",
                TongTienMatHeThong = ca.TongThuTienMat,
                TongVnpayHeThong = ca.TongThuVNPAY,
                TienMatLech = (ca.TienMatThucTe ?? 0) - (ca.TongThuTienMat + (ca.TienDauCa ?? 0))
            });
        }

       
    }
}
