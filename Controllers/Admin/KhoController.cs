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
        public async Task<IActionResult> GetByShop( int id)
        {
            var list= await _context.tonKhos
                .Include(tk=>tk.NguyenLieu)
                .Include(tk=>tk.CuaHang)
                .Where(tk=>tk.ShopId==id).ToListAsync();
            return Ok(list);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMaterial()
        {
            var list = await _context.NguyenLieu
                .Include(c=>c.TheLoai)
                .Include(c=>c.DoiTac)
                .ToListAsync();
            return Ok(list);
        }

        [HttpPost("nhap_kho")]
        public async Task<IActionResult> NhapKhoList([FromBody] NhapKhoDTO request)
        {
            using var transaction=_context.Database.BeginTransaction();
            try
            {
                //bieen lai
                var bienlai = new BienLai
                {
                    HanhDong = request.HanhDong,
                    CuaHangId = (int) request.KhoNhapId,
                    // FE nếu hd là nhập ncc thì hiện danh sách ncc , DIèu chuyển nội bộ là ds ch
                    DoiTacId = request.HanhDong == "NHAP_NCC" ? request.DoiTacId : null,
                    KhoXuatId = request.HanhDong == "NHAP_DIEU_CHUYEN_NOI_BO" ? request.KhoXuatId : null, 
                    NgayThucHien = DateTime.Now,
                    TrangThai="CHO_XAC_NHAN"
                };
                _context.BienLai.Add(bienlai);
                await _context.SaveChangesAsync();

                foreach (var item in request.Items) 
                {
                    var nguyenlieu = await _context.NguyenLieu
                            .FirstOrDefaultAsync(nl => nl.NguyenLieuName == item.NguyenLieuName.Trim() && nl.DoiTacId == request.DoiTacId);
                    if(nguyenlieu == null)
                    {
                        nguyenlieu = new NguyenLieu
                        {
                            NguyenLieuName = item.NguyenLieuName,
                            DonVi = item.DonVi,
                            GiaNhap = item.GiaNhap,
                            TheLoaiId = item.TheLoaiId,
                            DoiTacId = request.DoiTacId,
                            NgaySanXuat= item.NgaySanXuat,
                            HanSuDung= item.HanSuDung,
                            IsActive = true
                        };
                        _context.NguyenLieu.Add(nguyenlieu);
                        await _context.SaveChangesAsync();
                    }

                    var chiTiet = new ChiTietBienLai
                    {
                        BienLaiId = bienlai.Id,
                        NguyenLieuId = nguyenlieu.NguyenLieuId,
                        Soluong = item.SoLuong,
                        GhiChu = item.GhiChu
                    };
                    _context.ChiTietBienLai.Add(chiTiet);
                   

                    var KhoNhan = await _context.tonKhos
                        .FirstOrDefaultAsync(tk=>tk.ShopId==request.KhoNhapId && tk.NguyenLieuId ==nguyenlieu.NguyenLieuId);
                    if (KhoNhan == null)
                    {
                        KhoNhan = new TonKho
                        {
                            ShopId = (int) request.KhoNhapId,
                            NguyenLieuId = nguyenlieu.NguyenLieuId,
                            SoLuong = item.SoLuong
                        };
                        _context.tonKhos.Add(KhoNhan);
                        
                    }
                    else
                    {
                        KhoNhan.SoLuong += item.SoLuong;
                    }
 
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Nhập kho thanh công lô hàng");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest("Lỗi Nhập Kho :" + ex.Message);
            }
        }
        [HttpPost("xuat_kho")]
        public async Task<IActionResult> XuatKhoList([FromBody] NhapKhoDTO request)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                //bieen lai
                var bienlai = new BienLai
                {
                    HanhDong = request.HanhDong,
                    CuaHangId = (int) request.KhoXuatId,
                    // FE nếu hd là nhập ncc thì hiện danh sách ncc , DIèu chuyển nội bộ là ds ch
                    
                    DoiTacId = request.HanhDong == "XUAT_TRA_NCC" || request.HanhDong=="XUAT_HUY" ? request.DoiTacId : null,
                    KhoXuatId = request.HanhDong == "XUAT_DIEU_CHUYEN_NOI_BO" || request.HanhDong == "XUAT_HUY" ? request.KhoXuatId : null,
                    NgayThucHien = DateTime.Now,
                    TrangThai = "CHO_XAC_NHAN"
                };
                _context.BienLai.Add(bienlai);
                await _context.SaveChangesAsync();

                foreach (var item in request.Items)
                {
                    var nguyenlieu = await _context.NguyenLieu
                            .FirstOrDefaultAsync(nl => nl.NguyenLieuName == item.NguyenLieuName.Trim() && nl.DoiTacId == request.DoiTacId);
                    if (nguyenlieu == null)
                    {
                        nguyenlieu = new NguyenLieu
                        {
                            NguyenLieuName = item.NguyenLieuName,
                            DonVi = item.DonVi,
                            GiaNhap = item.GiaNhap,
                            TheLoaiId = item.TheLoaiId,
                            DoiTacId = request.DoiTacId,
                            NgaySanXuat = item.NgaySanXuat,
                            HanSuDung = item.HanSuDung,
                            IsActive = true
                        };
                        _context.NguyenLieu.Add(nguyenlieu);
                        await _context.SaveChangesAsync();
                    }

                    var chiTiet = new ChiTietBienLai
                    {
                        BienLaiId = bienlai.Id,
                        NguyenLieuId = nguyenlieu.NguyenLieuId,
                        Soluong = item.SoLuong,
                        GhiChu = item.GhiChu
                    };
                    _context.ChiTietBienLai.Add(chiTiet);


                    var KhoNhan = await _context.tonKhos
                        .FirstOrDefaultAsync(tk => tk.ShopId == request.KhoNhapId && tk.NguyenLieuId == nguyenlieu.NguyenLieuId);
                    if (KhoNhan == null)
                    {
                        KhoNhan = new TonKho
                        {
                            ShopId = (int) request.KhoNhapId,
                            NguyenLieuId = nguyenlieu.NguyenLieuId,
                            SoLuong = item.SoLuong
                        };
                        _context.tonKhos.Add(KhoNhan);

                    }
                    else
                    {
                        KhoNhan.SoLuong -= item.SoLuong;
                    }

                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Xuất kho thanh công lô hàng");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest("Lỗi Xuất Kho :" + ex.Message);
            }          
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

                var tonkho= await _context.tonKhos
                    .Where(tk=>tk.ShopId==request.ShopId)
                    .ToListAsync();

                var listChiTiet=new List<ChiTietKiemKe>();

                foreach( var item in request.ChiTiet)
                {
                    var tonkhoDb= tonkho.FirstOrDefault(tk=>tk.NguyenLieuId==item.NguyenLieuId);

                    float tonHeThong = tonkhoDb != null ? tonkhoDb.SoLuong : 0; // Nếu không tìm thấy nghĩa là tồn = 0
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
                        // Nếu thấy hàng lạ chưa từng nhập -> Tạo mới tồn kho
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
                return Ok("Kiểm kê thành công ");
            }
            catch (Exception ex) {
                await transaction.RollbackAsync();
                return BadRequest("Lỗi kiểm kê "+ex.Message);
            }
            
        }
    }
}
