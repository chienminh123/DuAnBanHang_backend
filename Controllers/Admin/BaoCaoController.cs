using backend.Data;
using backend.DTOs.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class BaoCaoController : ControllerBase
    {

        private readonly ShopContext _context;
        
        public BaoCaoController(ShopContext context)
        {
            _context = context;
        }

        [HttpPost("doanh-thu")]
        public async Task<IActionResult> GetDoanhThuDong([FromBody] BaoCaoDTO request)
        {
            DateTime thoiDiemKetThuc = request.EndDate.Date.AddDays(1).AddTicks(-1);

            var result = await _context.Order
                    .Where(o=> request.ShopIds.Contains(o.ShopId) 
                        && o.NgayTao>=request.StartDate.Date
                        && o.NgayTao<=thoiDiemKetThuc
                        && (o.TrangThaiDonHang=="DA_THANH_TOAN"||o.TrangThaiDonHang=="HOAN_THANH"))
                    .GroupBy(o=>o.NgayTao.Date)
                    .Select(g=> new { Ngay=g.Key,TongTien=g.Sum(x=>x.ThanhTien)})
                    .OrderBy(o=>o.Ngay)
                .ToListAsync();

            return Ok(result);
        }

        [HttpPost("top-mon")]
        public async Task<IActionResult> GetTopMon([FromBody] BaoCaoDTO request)
        {
            DateTime thoiDiemKetThuc = request.EndDate.Date.AddDays(1).AddTicks(-1);

            var result = await _context.OrderDetail
                .Include(od => od.Order)
                .Include(od=>od.SanPham)
                .Where(o=>request.ShopIds.Contains(o.Order.ShopId)
                    && o.Order.NgayTao>= request.StartDate.Date
                    && o.Order.NgayTao <= thoiDiemKetThuc
                    && (o.Order.TrangThaiDonHang == "DA_THANH_TOAN" || o.Order.TrangThaiDonHang == "HOAN_THANH"))
                .GroupBy(o=> new {o.SanPhamId , o.SanPham.SanPhamName})
                .Select(g => new {SanPhamId = g.Key, TenMon=g.Key.SanPhamName, TongSoLuong= g.Sum(x=>x.SoLuong) })
                .OrderByDescending(o=>o.TongSoLuong)
                .Take(5)
                .ToListAsync();

            return Ok(result);
        }

        [HttpPost("chi-tiet-nhap")]
        public async Task<IActionResult> GetChiTietNhapKho([FromBody] BaoCaoDTO request)
        {
            DateTime thoiDiemKetThuc = request.EndDate.Date.AddDays(1).AddTicks(-1);

            var result = await _context.ChiTietBienLai
                .Include(ct => ct.BienLai)
                .Include(ct => ct.NguyenLieu)
                .Where(b=> request.ShopIds.Contains(b.BienLai.CuaHangId)
                    && b.BienLai.NgayThucHien>=request.StartDate.Date
                    && b.BienLai.NgayThucHien<=thoiDiemKetThuc
                    &&( b.BienLai.HanhDong== "NHAP_NCC" || b.BienLai.HanhDong== "NHAP_DIEU_CHUYEN_NOI_BO")
                    && b.BienLai.TrangThai=="HOAN_THANH")
                .Select(g=> new {   MaPhieu=g.BienLai.Id,
                                    NgayThucHien=g.BienLai.NgayThucHien,
                                    TenNguyenLieu=g.NguyenLieu.NguyenLieuName,
                                    SoLuong=g.Soluong,
                                    DonVi=g.NguyenLieu.DonVi})
                .OrderByDescending(b=>b.NgayThucHien)
                .ToListAsync();

            return Ok(result);
        }

        [HttpPost("chi-tiet-xuat")]
        public async Task<IActionResult> GetChiTietXuatKho([FromBody] BaoCaoDTO request)
        {
            DateTime thoiDiemKetThuc = request.EndDate.Date.AddDays(1).AddTicks(-1);

            var result = await _context.ChiTietBienLai
                .Include(ct => ct.BienLai)
                .Include(ct => ct.NguyenLieu)
                .Where(b => request.ShopIds.Contains(b.BienLai.CuaHangId)
                    && b.BienLai.NgayThucHien >= request.StartDate.Date
                    && b.BienLai.NgayThucHien <= thoiDiemKetThuc
                    && (b.BienLai.HanhDong == "XUAT_HUY" || b.BienLai.HanhDong == "XUAT_DIEU_CHUYEN_NOI_BO" || b.BienLai.HanhDong == "XUAT_BAN")
                    && b.BienLai.TrangThai == "HOAN_THANH")
                .OrderByDescending(b => b.BienLai.NgayThucHien)
                .Select(g=> new {
                    MaPhieu = g.BienLai.Id,
                    MaMon = g.NguyenLieuId,
                    TenNguyenLieu = g.NguyenLieu.NguyenLieuName,
                    SoLuong = g.Soluong,
                    GhiChu = g.GhiChu
                })
                
                .ToListAsync();

            return Ok(result);
        }

        [HttpPost("lich-su-ca")]
        public async Task<IActionResult> GetLichSuCa([FromBody] BaoCaoDTO request)
        {
            DateTime thoiDiemKetThuc = request.EndDate.Date.AddDays(1).AddTicks(-1);

            var result = await _context.ChotCa 
                .Where(c=> request.ShopIds.Contains(c.ShopId)
                    && c.ThoiGianMo>=request.StartDate.Date
                    && c.ThoiGianMo<=thoiDiemKetThuc
                    && c.TrangThai=="DA_CHOT")
                .Select(g=> new { CaId = g.Id,
                                  ThoiGianMo=g.ThoiGianMo,
                                  ThoiGianDong=g.ThoiGianDong,
                                  TienHeThong=g.TongThuTienMat+(g.TienDauCa ?? 0),
                                  TienThucTe=g.TienMatThucTe,
                                  TienLech=(g.TienMatThucTe ?? 0 )-(g.TongThuTienMat + (g.TienDauCa ?? 0)),
                                  GhiChu=g.GhiChu
                })
                .OrderByDescending(c=>c.ThoiGianMo)
                .ToListAsync();

            return Ok(result);
        }
    }
}
