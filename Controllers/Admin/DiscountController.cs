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
    public class DiscountController : ControllerBase
    {
        private readonly ShopContext _context;
        public DiscountController(ShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.MaGiamGia.ToListAsync();
            return Ok(list);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveDiscount()
        {
            var today = DateTime.Now;
            var activeDiscounts = await _context.MaGiamGia
                .Where(d => d.IsActive == true
                        && d.SoLuong > 0
                        && d.NgayBatDau <= today
                        && d.NgayKetThuc >= today)
                .Select(g => new
                {
                    g.Code,
                    g.LoaiMa,
                    g.MaxValue
                }).ToListAsync();
            return Ok(activeDiscounts);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var discount=await _context.MaGiamGia.FirstOrDefaultAsync(dc=>dc.Id==id);
            if (discount == null) {
                return NotFound("K tồn tại mã giảm giá này ");
            }
            _context.MaGiamGia.Remove(discount);
            _context.SaveChanges();
            return Ok("Xóa thành công  mã giảm giá ");
        }

        [HttpPut("discount-update/{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] DiscountDTO request)
        {
            var discount = await _context.MaGiamGia.FirstOrDefaultAsync(dc => dc.Id == id);
            if (discount == null)
            {
                return NotFound("K tồn tại mã giảm giá này ");
            }
            discount.Code = !string.IsNullOrEmpty(request.Code) ? request.Code : discount.Code;
            discount.LoaiMa = !string.IsNullOrEmpty(request.LoaiMa) ? request.LoaiMa : discount.LoaiMa;
            discount.MaxValue = request.MaxValue ?? discount.MaxValue;
            discount.SoLuong= request.SoLuong ?? discount.SoLuong;
            discount.NgayBatDau= request.NgayBatDau ?? discount.NgayBatDau;
            discount.NgayKetThuc= request.NgayKetThuc ?? discount.NgayKetThuc;
            await _context.SaveChangesAsync();
            return Ok("cập nhật thành công ma giảm giá");
        }

        [HttpPatch("update-status/{id}")] 
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var discount = await _context.MaGiamGia.FindAsync(id);
            if (discount == null) return NotFound("Không tìm thấy mã giảm giá");
            discount.IsActive = !discount.IsActive;
            await _context.SaveChangesAsync();
            return Ok(new
            {
                Message = "Cập nhật thành công",
                NewStatus = discount.IsActive
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm]DiscountDTO request)
        {
            var discount= await _context.MaGiamGia.FirstOrDefaultAsync(d=>d.Code== request.Code.Trim());
            if (discount != null)
            {
                return BadRequest("mã giảm giá này đã tồn tại ");
            }

            var newDiscount = new MaGiamGia
            {
                Code = request.Code,
                LoaiMa = request.LoaiMa,
                DiscountValue = request.DiscountValue,
                MaxValue = request.MaxValue ?? 0,
                SoLuong = request.SoLuong ?? 10,
                NgayBatDau = request.NgayBatDau ?? DateTime.Now.AddDays(1),
                NgayKetThuc = request.NgayKetThuc ?? DateTime.Now.AddDays(7),
                IsActive = true
            };
            _context.MaGiamGia.Add(newDiscount);
            await _context.SaveChangesAsync();
            return Ok("taoj thanh coong mã giảm giá ");
        }

        [HttpGet("search-discount-by-code")]
        public async Task<IActionResult> SearchByCode([FromQuery]string code)
        {
            if(string.IsNullOrEmpty(code)) return await GetAll();
            var discount= await _context.MaGiamGia.Where(d=>d.Code.Contains(code)).ToListAsync();
            if (discount == null)
            {
                return NotFound("k tìm thấy mã phù hợp ");
            }
            return Ok(discount);
        }

        [HttpGet("search-discount-by-date")]
        public async Task<IActionResult> SearchByDate([FromQuery]DateTime date1,[FromQuery]DateTime date2)
        {
            var discount=await _context.MaGiamGia.Where(d=>d.NgayBatDau>=date1 && d.NgayKetThuc<=date2).ToListAsync();
            if (discount == null)
            {
                NotFound("k tìm thấy mã nào trong khoảng thời gian này ");
            }
            return Ok(discount);
        }

        [HttpGet("check")]
        public async Task<IActionResult> CheckVoucher([FromQuery] string code, [FromQuery] int shopId, [FromQuery] double totalAmount)
        {
            var discount = await _context.MaGiamGia.FirstOrDefaultAsync(d => d.Code == code.Trim());

            if (discount == null || !discount.IsActive || discount.SoLuong <= 0)
                return Ok(new { isValid = false, message = "Mã không khả dụng!" });

            double soTienGiam = 0;

            if (discount.LoaiMa.ToUpper() == "PERCENT")
            {
                soTienGiam = totalAmount * (discount.DiscountValue / 100.0);

                if (soTienGiam > discount.MaxValue) soTienGiam = discount.MaxValue;
            }
            else if (discount.LoaiMa.ToUpper() == "FIXED")
            {
                soTienGiam = discount.DiscountValue;
            }

            if (soTienGiam > totalAmount) soTienGiam = totalAmount;

            return Ok(new
            {
                isValid = true,
                soTienGiam = soTienGiam,
                message = "Áp dụng thành công!"
            });
        }
    }
}
