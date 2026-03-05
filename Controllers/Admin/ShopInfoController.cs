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
                ShopCity=request.city
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

            if(!string.IsNullOrEmpty(request.name)) sp.ShopName= request.name;
            if(!string.IsNullOrEmpty(request.phone) && CheckPhone(request.phone)==true) sp.ShopPhone= request.phone;
            if(!string.IsNullOrEmpty(request.address)) sp.ShopAddress= request.address;
            if(!string.IsNullOrEmpty(request.city)) sp.ShopCity= request.city;

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
