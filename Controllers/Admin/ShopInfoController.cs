using backend.Data;
using backend.DTOs.Admin;
using backend.Models.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace backend.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class ShopInfoController : ControllerBase
    {
        private readonly ShopContext _context;

        public ShopInfoController(ShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.cuaHangs
                    .ToListAsync();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ShopInfoDTO request)
        {
            var ch= await _context.cuaHangs.FirstOrDefaultAsync(s=>s.ShopName==request.name);
            if (ch != null)
            {
                return BadRequest("Cửa hàng này đã tồn tại");
            }
            if (CheckPhone(request.phone) == false) return BadRequest("sdt k đúng định dạng");
            var sp = new CuaHang
            {
                ShopName=request.name,
                ShopPhone=request.phone,
                ShopAddress=request.address,
                ShopCity=request.city,
                Latitude = request.latitude,
                Longitude = request.longitude,
                BanKinhChoPhep = request.BanKinhChoPhep > 0 ? request.BanKinhChoPhep : 50
            };

            _context.cuaHangs.Add(sp);
            await _context.SaveChangesAsync();
            return Ok($"Thêm cửa hàng thành công ! {sp}");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var sp = await _context.cuaHangs.FindAsync(id);
            if (sp == null)
            {
                return NotFound("Không tìm thấy cửa hàng!");
            }
            
            _context.cuaHangs.Remove(sp);
            await _context.SaveChangesAsync();
            return Ok($"Đã xóa cửa hàng mã {id}");
        }

        [HttpPut("shop-update/{id}")]
        public async Task<IActionResult> Update(int id,ShopInfoDTO request)
        {
            var sp = await _context.cuaHangs.FindAsync(id);
            if (sp == null)
            {
                return NotFound("k tìm thấy cửa hàng cần sửa");
            }

            sp.ShopName=!string.IsNullOrEmpty(request.name) ? request.name : sp.ShopName;
            sp.ShopPhone = !string.IsNullOrEmpty(request.phone) && CheckPhone(request.phone) == true ? request.phone : sp.ShopPhone;
            sp.ShopAddress = !string.IsNullOrEmpty(request.address) ? request.address : sp.ShopAddress;
            sp.ShopCity = !string.IsNullOrEmpty(request.city) ? request.city : sp.ShopCity;
            if (request.latitude.HasValue) sp.Latitude = request.latitude;
            if (request.longitude.HasValue) sp.Longitude = request.longitude;
            if (request.BanKinhChoPhep > 0) sp.BanKinhChoPhep = request.BanKinhChoPhep;

            await _context.SaveChangesAsync();
            return Ok("sửa thông tin thành công");
        }

        [HttpGet("search-by-name")]
        public async Task<IActionResult> SearchByName([FromQuery]string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return await GetAll();
            }
            var result = await _context.cuaHangs.Where(s=>s.ShopName.Contains(key)).ToListAsync();
            if (result.Count == 0)
            {
                return NotFound("k tìm thấy cửa hàng nào khớp với từ khóa ");
            }
            return Ok(result);
        }

        private bool CheckPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return false;

            string pattern = @"^(0)(3|5|7|8|9)[0-9]{8}$";
            return Regex.IsMatch(phone, pattern);
        }
    }
}
