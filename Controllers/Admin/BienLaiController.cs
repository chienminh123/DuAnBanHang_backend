using backend.Data;
using backend.Models.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.Admin
{
    [Route("api/Admin/[controller]")]
    [ApiController]
    public class BienLaiController : ControllerBase
    {

        private readonly ShopContext _context;
        
        public BienLaiController(ShopContext context)
        {
            _context = context;
        }

        [HttpGet("danh_sach_phieu")]
        public async Task<IActionResult> GetDanhSachPhieu([FromQuery] string hanhDong, [FromQuery] int shopId)
        {
            var list = await _context.BienLai
                .Where(b => b.HanhDong == hanhDong && b.CuaHangId == shopId)
                .OrderByDescending(b => b.NgayThucHien)
                .Select(b => new {
                    b.Id,
                    b.HanhDong,
                    b.NgayThucHien,
                    b.TrangThai,
                    TenCuaHang = b.CuaHang.ShopName,
                    TenDoiTac = b.DoiTac != null ? b.DoiTac.Name : "Nội bộ",
                    KhoXuatId = b.KhoXuatId
                })
                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("chi_tiet_phieu/{bienLaiId}")]
        public async Task<IActionResult> GetChiTietPhieu(int bienLaiId)
        {
            var details = await _context.ChiTietBienLai
                .Include(ct => ct.NguyenLieu)
                .Where(ct => ct.BienLaiId == bienLaiId)
                .Select(ct => new {
                    ct.Id,
                    TenNguyenLieu = ct.NguyenLieu.NguyenLieuName,
                    DonVi = ct.NguyenLieu.DonVi,
                    ct.Soluong,
                    ct.GhiChu
                })
                .ToListAsync();
            return Ok(details);
        }

        [HttpPut("xu_ly_phieu/{bienLaiId}")]
        public async Task<IActionResult> XuLyPhieu(int bienLaiId, [FromQuery] string trangThaiMoi)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var bienLai = await _context.BienLai.FindAsync(bienLaiId);
                if (bienLai == null) return NotFound("Không tìm thấy mã phiếu!");
                if (bienLai.TrangThai != "CHO_XAC_NHAN") return BadRequest("Phiếu này đã được xử lý!");

                bienLai.TrangThai = trangThaiMoi;
                var chiTiets = await _context.ChiTietBienLai.Where(c => c.BienLaiId == bienLaiId).ToListAsync();

                // NẾU LÀ HOÀN THÀNH
                if (trangThaiMoi == "HOAN_THANH")
                {
                    foreach (var item in chiTiets)
                    {
                        var tonKho = await _context.tonKhos.FirstOrDefaultAsync(tk => tk.ShopId == bienLai.CuaHangId && tk.NguyenLieuId == item.NguyenLieuId);

                        if (bienLai.HanhDong.Contains("NHAP"))
                        {
                            if (tonKho == null) _context.tonKhos.Add(new TonKho { ShopId = bienLai.CuaHangId, NguyenLieuId = item.NguyenLieuId, SoLuong = item.Soluong });
                            else tonKho.SoLuong += item.Soluong;
                        }
                        else if (bienLai.HanhDong == "XUAT_TRA_NCC")
                        {
                            if (tonKho != null) { tonKho.SoLuong -= item.Soluong; if (tonKho.SoLuong < 0) tonKho.SoLuong = 0; }
                        }
                    }
                }
                // NẾU BẤM HỦY
                else if (trangThaiMoi == "DA_HUY")
                {
                    if (bienLai.HanhDong == "NHAP_DIEU_CHUYEN_NOI_BO")
                    {
                        foreach (var item in chiTiets)
                        {
                            var tonKhoNguon = await _context.tonKhos.FirstOrDefaultAsync(tk => tk.ShopId == bienLai.KhoXuatId && tk.NguyenLieuId == item.NguyenLieuId);
                            if (tonKhoNguon != null) tonKhoNguon.SoLuong += item.Soluong;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Xử lý phiếu thành công!");
            }
            catch (Exception ex) { await transaction.RollbackAsync(); return StatusCode(500, "Lỗi: " + ex.Message); }
        }
    }
}
