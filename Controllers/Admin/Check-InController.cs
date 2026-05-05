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
    public class Check_InController : ControllerBase
    {
        private readonly ShopContext _context;

        public Check_InController(ShopContext context)
        {
            _context = context;
        }

        [HttpGet("get_history")]
        public async Task<IActionResult> GetHistory([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            // Join 3 bảng: ChamCong + TaiKhoanNoiBo + CaLamViec để lấy Tên NV và Tên Ca
            var query = from cc in _context.ChamCong
                        join nv in _context.TaiKhoanNoiBo on cc.TaiKhanNoiBoId equals nv.TaiKhoanNoiBoId
                        join ca in _context.CaLamViec on cc.CaLamViecId equals ca.Id
                        where cc.Ngay >= startDate.Date && cc.Ngay <= endDate.Date
                        orderby cc.Ngay descending, cc.GioVao descending
                        select new
                        {
                            cc.Id,
                            TenNhanVien = nv.TenNhanVien,
                            TenCa = ca.TenCa,
                            cc.Ngay,
                            cc.GioVao,
                            cc.GioRa,
                            cc.TrangThai,
                            cc.GhiChu
                        };

            var result = await query.ToListAsync();
            return Ok(result);
        }

        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] Check_InDTO request)
        {
            var nhanVien = await _context.TaiKhoanNoiBo.FindAsync(request.TaiKhoanNoiBoId);
            var cuaHang = await _context.cuaHangs.FindAsync(request.ShopId);
            var caLam = await _context.CaLamViec.FindAsync(request.CaLamViecId);

            if (nhanVien == null || cuaHang == null || caLam == null)
                return BadRequest("Thông tin nhân viên, cửa hàng hoặc ca làm việc không hợp lệ.");

            if (!cuaHang.Latitude.HasValue || !cuaHang.Longitude.HasValue)
                return BadRequest("Cửa hàng chưa được thiết lập tọa độ GPS. Vui lòng liên hệ Admin.");

            double distance = backend.Helpers.LocationHelper.CalculateDistance(
                request.Latitude, request.Longitude,
                cuaHang.Latitude.Value, cuaHang.Longitude.Value
            );

            if (distance > cuaHang.BanKinhChoPhep)
            {
                return BadRequest($"Bạn đang ở cách cửa hàng {Math.Round(distance)}m. Vui lòng vào trong bán kính {cuaHang.BanKinhChoPhep}m để chấm công.");
            }

            var homNay = DateTime.Today;
            var gioHienTai = DateTime.Now.TimeOfDay;

            var chamCongDb = await _context.ChamCong.FirstOrDefaultAsync(cc =>
                cc.TaiKhanNoiBoId == request.TaiKhoanNoiBoId &&
                cc.CaLamViecId == request.CaLamViecId &&
                cc.Ngay.Date == homNay);

            if (chamCongDb == null)
            {
                // LẦN 1: CHECK-IN (LƯU GIỜ VÀO)
                string trangThai = "Đúng giờ";
                if (gioHienTai > caLam.GioBatDau)
                {
                    var muon = gioHienTai - caLam.GioBatDau;
                    trangThai = $"Đi muộn {(int)muon.TotalMinutes} phút";
                }

                var chamCongMoi = new ChamCong
                {
                    TaiKhanNoiBoId = request.TaiKhoanNoiBoId,
                    CaLamViecId = request.CaLamViecId,
                    Ngay = homNay,
                    GioVao = DateTime.Now,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    TrangThai = trangThai,
                    GhiChu = $"Vào: Cách {Math.Round(distance, 1)}m"
                };

                _context.ChamCong.Add(chamCongMoi);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Check-in thành công!",
                    time = chamCongMoi.GioVao?.ToString("HH:mm:ss"),
                    status = trangThai
                });
            }
            else
            {
                // LẦN 2: CHECK-OUT (LƯU GIỜ RA)
                chamCongDb.GioRa = DateTime.Now;

                chamCongDb.GhiChu += $" | Ra: Cách {Math.Round(distance, 1)}m";

                // Kiểm tra xem có về sớm trước giờ kết thúc ca không
                if (gioHienTai < caLam.GioKetThuc)
                {
                    var som = caLam.GioKetThuc - gioHienTai;
                    // Nếu trạng thái chưa có chữ "Về sớm" thì nối thêm vào
                    if (!chamCongDb.TrangThai.Contains("Về sớm"))
                    {
                        chamCongDb.TrangThai += $", Về sớm {(int)som.TotalMinutes} phút";
                    }
                }

                _context.ChamCong.Update(chamCongDb);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Check-out thành công!",
                    time = chamCongDb.GioRa?.ToString("HH:mm:ss"),
                    status = "Đã hoàn thành ca"
                });
            }
        }

        
    }
}

