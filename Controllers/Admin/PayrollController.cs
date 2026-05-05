using backend.Data;
using backend.DTOs.Admin;
using backend.Models.Admin;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class PayrollController : ControllerBase
    {
        private readonly ShopContext _context;
        private readonly EmailService _emailService;
        public PayrollController(ShopContext context,EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // CẤU HÌNH LƯƠNG
        [HttpGet("config")]
        public async Task<IActionResult> GetConfig()
        {
            var configs = await _context.CauHinhLuong
                .Include(c => c.TaiKhoanNoiBo) 
                .Select(c => new {
                    c.Id,
                    c.TaiKhoanNoiBoId,
                    TenNhanVien = c.TaiKhoanNoiBo.TenNhanVien,
                    c.LoaiLuong,
                    c.MucLuong,
                    c.PhuCapAnTrua,
                    c.PhuCapXangXe,
                    c.PhuCapChuyenCan
                }).ToListAsync();
            return Ok(configs);
        }

        [HttpPost("config")]
        public async Task<IActionResult> SaveConfig([FromBody] CauHinhLuongDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existing = await _context.CauHinhLuong.FirstOrDefaultAsync(c => c.TaiKhoanNoiBoId == request.TaiKhoanNoiBoId);

            if (existing != null)
            {
                existing.LoaiLuong = request.LoaiLuong;
                existing.MucLuong = request.MucLuong;
                existing.PhuCapAnTrua = request.PhuCapAnTrua;
                existing.PhuCapXangXe = request.PhuCapXangXe;
                existing.PhuCapChuyenCan = request.PhuCapChuyenCan;
                _context.CauHinhLuong.Update(existing);
            }
            else
            {
                var newConfig = new CauHinhLuong
                {
                    TaiKhoanNoiBoId = request.TaiKhoanNoiBoId,
                    LoaiLuong = request.LoaiLuong,
                    MucLuong = request.MucLuong,
                    PhuCapAnTrua = request.PhuCapAnTrua,
                    PhuCapXangXe = request.PhuCapXangXe,
                    PhuCapChuyenCan = request.PhuCapChuyenCan
                };
                _context.CauHinhLuong.Add(newConfig);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Lưu cấu hình lương thành công!" });
        }

        // TỰ ĐỘNG TÍNH LƯƠNG THÁNG
        [HttpPost("calculate")]
        public async Task<IActionResult> CalculatePayroll([FromQuery] int thang, [FromQuery] int nam)
        {
            var configs = await _context.CauHinhLuong.Include(c => c.TaiKhoanNoiBo).ToListAsync();
            if (!configs.Any()) return BadRequest("Chưa có cấu hình lương cho nhân viên nào!");

            var chamCongs = await _context.ChamCong
                .Where(cc => cc.Ngay.Month == thang && cc.Ngay.Year == nam)
                .ToListAsync();

            var oldPayrolls = await _context.BangLuong
                .Where(b => b.Thang == thang && b.Nam == nam && b.TrangThai == "CHUA_THANH_TOAN")
                .ToListAsync();

            if (oldPayrolls.Any())
            {
                _context.BangLuong.RemoveRange(oldPayrolls);
                await _context.SaveChangesAsync();
            }

            var newPayrolls = new List<BangLuong>();

            foreach (var config in configs)
            {
                var ccNhanVien = chamCongs.Where(cc => cc.TaiKhanNoiBoId == config.TaiKhoanNoiBoId).ToList();

                if (!ccNhanVien.Any()) continue;

                double tongGioLam = 0;
                double tongNgayLam = 0; 
                double tienPhat = 0;

                foreach (var cc in ccNhanVien)
                {
                    if (cc.GioVao.HasValue && cc.GioRa.HasValue)
                    {
                        tongGioLam += (cc.GioRa.Value - cc.GioVao.Value).TotalHours;
                        tongNgayLam += 0.5;
                    }

                    if (!string.IsNullOrEmpty(cc.TrangThai) && cc.TrangThai.ToLower().Contains("muộn"))
                    {
                        tienPhat += 5000;
                    }
                }

                double tiLeNgay = tongNgayLam >= 26 ? 1 : (tongNgayLam / 26.0);

                double anTruaThucTe = config.PhuCapAnTrua * tiLeNgay;
                double xangXeThucTe = config.PhuCapXangXe * tiLeNgay;

                // Mất 100% chuyên cần nếu nghỉ ngày nào (tổng ngày < 26) HOẶC đi muộn
                double chuyenCanThucTe = (tongNgayLam >= 26 && tienPhat == 0) ? config.PhuCapChuyenCan : 0;

                double tongPhuCapThucTe = anTruaThucTe + xangXeThucTe + chuyenCanThucTe;

                double luongThucLanh = 0;

                if (config.LoaiLuong == "THEO_GIO")
                {
                    luongThucLanh = (tongGioLam * config.MucLuong) + tongPhuCapThucTe - tienPhat;
                }
                else // CO_DINH
                {
                    luongThucLanh = (config.MucLuong * tiLeNgay) + tongPhuCapThucTe - tienPhat;
                }

                newPayrolls.Add(new BangLuong
                {
                    TaiKhoanNoiBoId = config.TaiKhoanNoiBoId,
                    Thang = thang,
                    Nam = nam,
                    TongGioLam = Math.Round(tongGioLam, 1),
                    TongNgayLam = tongNgayLam,
                    TienPhatDiMuon = tienPhat,
                    PhuCapAnTrua = Math.Round(anTruaThucTe),
                    PhuCapXangXe = Math.Round(xangXeThucTe),
                    PhuCapChuyenCan = Math.Round(chuyenCanThucTe),
                    TongPhuCap = Math.Round(tongPhuCapThucTe),

                    TongTienNhan = luongThucLanh > 0 ? Math.Round(luongThucLanh) : 0,
                    TrangThai = "CHUA_THANH_TOAN"
                });
            }

            _context.BangLuong.AddRange(newPayrolls);
            await _context.SaveChangesAsync();

            var emailTasks = new List<Task>();

            foreach (var payroll in newPayrolls)
            {
                // Tìm lại cấu hình của nhân viên này để lấy Email và định dạng gửi
                var config = configs.First(c => c.TaiKhoanNoiBoId == payroll.TaiKhoanNoiBoId);

                if (!string.IsNullOrEmpty(config.TaiKhoanNoiBo?.Email))
                {
                    string kieuTinhCong = config.LoaiLuong == "THEO_GIO"
                        ? $"{payroll.TongGioLam} giờ"
                        : $"{payroll.TongNgayLam} ngày";

                    emailTasks.Add(_emailService.SendPayslipEmailAsync(
                        config.TaiKhoanNoiBo.Email,
                        config.TaiKhoanNoiBo.TenNhanVien,
                        thang,
                        nam,
                        kieuTinhCong,
                        config.MucLuong.ToString("N0") + " VNĐ",
                        payroll.PhuCapAnTrua.ToString("N0") + " VNĐ",
                        payroll.PhuCapXangXe.ToString("N0") + " VNĐ",
                        payroll.PhuCapChuyenCan.ToString("N0") + " VNĐ",
                        payroll.TienPhatDiMuon.ToString("N0") + " VNĐ",
                        payroll.TongTienNhan.ToString("N0") + " VNĐ"
                    ));
                }
            }

            if (emailTasks.Any())
            {
                await Task.WhenAll(emailTasks);
            }

            return Ok(new { message = $"Đã chốt lương và gửi email cho nhân viên tháng {thang}/{nam} thành công!" });
        }

        // DANH SÁCH BẢNG LƯƠNG THEO THÁNG/NĂM
        [HttpGet("list")]
        public async Task<IActionResult> GetPayrolls([FromQuery] int thang, [FromQuery] int nam)
        {
            var payrolls = await _context.BangLuong
                .Include(b => b.TaiKhoanNoiBo) 
                .Where(b => b.Thang == thang && b.Nam == nam)
                .Select(b => new {
                    b.Id,
                    b.TaiKhoanNoiBoId,
                    TenNhanVien = b.TaiKhoanNoiBo.TenNhanVien,
                    b.Thang,
                    b.Nam,
                    b.TongGioLam,
                    b.TongNgayLam,
                    b.TienPhatDiMuon,
                    b.PhuCapAnTrua,
                    b.PhuCapXangXe,
                    b.PhuCapChuyenCan,
                    b.TongPhuCap,
                    b.TongTienNhan,
                    b.TrangThai
                }).ToListAsync();

            return Ok(payrolls);
        }

        //ĐÁNH DẤU THANH TOÁN
        [HttpPut("pay/{id}")]
        public async Task<IActionResult> MarkAsPaid(int id)
        {
            var payroll = await _context.BangLuong.FindAsync(id);
            if (payroll == null) return NotFound("Không tìm thấy phiếu lương");

            payroll.TrangThai = "DA_THANH_TOAN";
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã đánh dấu thanh toán!" });
        }
    }
}