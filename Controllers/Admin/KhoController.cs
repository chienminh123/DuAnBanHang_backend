using backend.Data;
using backend.DTOs.Admin;
using backend.Models.Admin;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.Admin
{
    [Route("api/Admin/[controller]")]
    [ApiController]
    public class KhoController : ControllerBase
    {
        private readonly ShopContext _context;

        public KhoController(ShopContext context)
        {
            _context = context;
        }

        [HttpGet("get_by_shop/{id}")]
        public async Task<IActionResult> GetByShop(int id)
        {
            var list = await _context.tonKhos
                .Include(tk => tk.NguyenLieu)
                .Include(tk => tk.CuaHang)
                .Where(tk => tk.ShopId == id).ToListAsync();
            return Ok(list);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMaterial()
        {
            var list = await _context.NguyenLieu
                .Include(c => c.TheLoai)
                .Include(c => c.DoiTac)
                .ToListAsync();
            return Ok(list);
        }

        [HttpPost("nhap_kho")]
        public async Task<IActionResult> NhapKhoList([FromBody] NhapKhoDTO request)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var bienlai = new BienLai
                {
                    HanhDong = request.HanhDong,
                    CuaHangId = (int)request.KhoNhapId,
                    DoiTacId = request.DoiTacId,
                    NgayThucHien = DateTime.Now,
                    TrangThai = "CHO_XAC_NHAN" 
                };
                _context.BienLai.Add(bienlai);
                await _context.SaveChangesAsync();

                foreach (var item in request.Items)
                {
                    var nguyenlieu = await _context.NguyenLieu.FirstOrDefaultAsync(nl => nl.NguyenLieuName == item.NguyenLieuName.Trim());
                    if (nguyenlieu == null)
                    {
                        nguyenlieu = new NguyenLieu
                        {
                            NguyenLieuName = item.NguyenLieuName,
                            DonVi = item.DonVi,
                            GiaNhap = item.GiaNhap,
                            TheLoaiId = item.TheLoaiId,
                            NgaySanXuat = item.NgaySanXuat,
                            HanSuDung = item.HanSuDung,
                            IsActive = true
                        };
                        _context.NguyenLieu.Add(nguyenlieu);
                        await _context.SaveChangesAsync();
                    }

                    _context.ChiTietBienLai.Add(new ChiTietBienLai { BienLaiId = bienlai.Id, NguyenLieuId = nguyenlieu.NguyenLieuId, Soluong = item.SoLuong, GhiChu = item.GhiChu });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Đã tạo Phiếu Nhập chờ xác nhận");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); return BadRequest("Lỗi: " + ex.Message);
            }
        }

        [HttpPost("xuat_kho")]
        public async Task<IActionResult> XuatKhoList([FromBody] NhapKhoDTO request)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // điều chuyển nội bộ
                if (request.HanhDong == "XUAT_DIEU_CHUYEN_NOI_BO")
                {
                    // Tạo Phiếu Xuất (Trừ kho ngay)
                    var bienLaiXuat = new BienLai
                    {
                        HanhDong = "XUAT_DIEU_CHUYEN_NOI_BO",
                        CuaHangId = (int)request.KhoXuatId, 
                        KhoXuatId = request.KhoNhapId,      
                        NgayThucHien = DateTime.Now,
                        TrangThai = "HOAN_THANH"
                    };
                    _context.BienLai.Add(bienLaiXuat);

                    var bienLaiNhap = new BienLai
                    {
                        HanhDong = "NHAP_DIEU_CHUYEN_NOI_BO",
                        CuaHangId = (int)request.KhoNhapId,
                        KhoXuatId = request.KhoXuatId,   
                        NgayThucHien = DateTime.Now,
                        TrangThai = "CHO_XAC_NHAN"
                    };
                    _context.BienLai.Add(bienLaiNhap);
                    await _context.SaveChangesAsync();

                    foreach (var item in request.Items)
                    {
                        var nguyenlieu = await _context.NguyenLieu.FirstOrDefaultAsync(nl => nl.NguyenLieuName == item.NguyenLieuName.Trim());
                        if (nguyenlieu != null)
                        {
                            _context.ChiTietBienLai.Add(new ChiTietBienLai { BienLaiId = bienLaiXuat.Id, NguyenLieuId = nguyenlieu.NguyenLieuId, Soluong = item.SoLuong, GhiChu = item.GhiChu });
                            _context.ChiTietBienLai.Add(new ChiTietBienLai { BienLaiId = bienLaiNhap.Id, NguyenLieuId = nguyenlieu.NguyenLieuId, Soluong = item.SoLuong, GhiChu = item.GhiChu });

                            
                            var khoXuat = await _context.tonKhos.FirstOrDefaultAsync(tk => tk.ShopId == request.KhoXuatId && tk.NguyenLieuId == nguyenlieu.NguyenLieuId);
                            if (khoXuat != null) { khoXuat.SoLuong -= item.SoLuong; if (khoXuat.SoLuong < 0) khoXuat.SoLuong = 0; }
                        }
                    }
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok("Đã xuất điều chuyển, trừ kho nguồn và tạo lệnh nhập chờ xác nhận tại kho đích.");
                }
                else
                {
                    //xuất hủy hoặc trả ncc
                    bool isXuatHuy = request.HanhDong == "XUAT_HUY";
                    var bienlai = new BienLai
                    {
                        HanhDong = request.HanhDong,
                        CuaHangId = (int)request.KhoXuatId,
                        DoiTacId = request.DoiTacId,
                        NgayThucHien = DateTime.Now,
                        TrangThai = isXuatHuy ? "HOAN_THANH" : "CHO_XAC_NHAN" 
                    };
                    _context.BienLai.Add(bienlai);
                    await _context.SaveChangesAsync();

                    foreach (var item in request.Items)
                    {
                        var nguyenlieu = await _context.NguyenLieu.FirstOrDefaultAsync(nl => nl.NguyenLieuName == item.NguyenLieuName.Trim());
                        if (nguyenlieu != null)
                        {
                            _context.ChiTietBienLai.Add(new ChiTietBienLai { BienLaiId = bienlai.Id, NguyenLieuId = nguyenlieu.NguyenLieuId, Soluong = item.SoLuong, GhiChu = item.GhiChu });

                            if (isXuatHuy)
                            {
                                var khoXuat = await _context.tonKhos.FirstOrDefaultAsync(tk => tk.ShopId == request.KhoXuatId && tk.NguyenLieuId == nguyenlieu.NguyenLieuId);
                                if (khoXuat != null) { khoXuat.SoLuong -= item.SoLuong; if (khoXuat.SoLuong < 0) khoXuat.SoLuong = 0; }
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok(isXuatHuy ? "Đã xuất hủy và trừ kho" : "Đã tạo phiếu chờ duyệt");
                }
            }
            catch (Exception ex) { await transaction.RollbackAsync(); return BadRequest("Lỗi: " + ex.Message); }
        }

        [HttpPost("kiem_ke")]
        public async Task<IActionResult> KiemKeList([FromBody] KiemKeDTO request)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var phieu = new KiemKe
                {
                    ShopId = request.ShopId,
                    NgayThucHien = DateTime.Now
                };
                _context.KiemKe.Add(phieu);
                await _context.SaveChangesAsync();

                var tonkho = await _context.tonKhos
                    .Where(tk => tk.ShopId == request.ShopId)
                    .ToListAsync();

                var listChiTiet = new List<ChiTietKiemKe>();

                foreach (var item in request.ChiTiet)
                {
                    var tonkhoDb = tonkho.FirstOrDefault(tk => tk.NguyenLieuId == item.NguyenLieuId);

                    float tonHeThong = tonkhoDb != null ? tonkhoDb.SoLuong : 0;
                    float chenhLech = item.TonThucTe - tonHeThong;

                    var chiTiet = new ChiTietKiemKe
                    {
                        KiemKeId = phieu.KiemKeId,
                        NguyenLieuId = item.NguyenLieuId,
                        TonHeThong = tonHeThong,
                        TonThucTe = item.TonThucTe,
                        ChenhLech = chenhLech,
                        Note = item.Note
                    };
                    listChiTiet.Add(chiTiet);

                    if (tonkhoDb != null)
                    {
                        tonkhoDb.SoLuong = item.TonThucTe;
                        _context.tonKhos.Update(tonkhoDb);
                    }
                    else
                    {
                        var newTonKho = new TonKho
                        {
                            ShopId = request.ShopId,
                            NguyenLieuId = item.NguyenLieuId,
                            SoLuong = item.TonThucTe
                        };
                        _context.tonKhos.Add(newTonKho);
                    }
                }
                _context.ChiTietKiemKe.AddRange(listChiTiet);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return Ok("Kiểm kê thành công");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest("Lỗi kiểm kê " + ex.Message);
            }
        }
    }
}