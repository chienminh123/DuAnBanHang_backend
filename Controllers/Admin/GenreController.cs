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
    public class GenreController : ControllerBase
    {
        private readonly ShopContext _context;

        public GenreController(ShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.theLoais.ToListAsync();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] GenreDTO request)
        {
            var list= await _context.theLoais.FirstOrDefaultAsync(s=>s.TheLoaiName==request.GenreName);
            if (list != null)
            {
                return BadRequest("Thể loại này đã tồn tại ");
            }

            var newGenre = new TheLoai
            {
                TheLoaiName = request.GenreName
            };

            _context.theLoais.Add(newGenre);
            await _context.SaveChangesAsync();
            return Ok("thêm thể loại thành công");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var gr= await _context.theLoais.FindAsync(id);
            if (gr == null)
            {
                return NotFound("k tìm thấy thể loại này ");
            }
            _context.theLoais.Remove(gr);
            await _context.SaveChangesAsync();
            return Ok("xóa thành công");
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromForm] GenreDTO request)
        {
            var sp = await _context.theLoais.FindAsync(id);
            if (sp == null)
            {
                return NotFound("k tìm thấy cửa hàng cần sửa");
            }
            sp.TheLoaiName = !string.IsNullOrEmpty(request.GenreName) ? request.GenreName : sp.TheLoaiName;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("search-by-name")]
        public async Task<IActionResult> SearchByName([FromQuery] string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return await GetAll();
            }
            var result = await _context.theLoais.Where(s => s.TheLoaiName.Contains(key)).ToListAsync();
            if (result.Count == 0)
            {
                return NotFound("k tìm thấy cửa hàng nào khớp với từ khóa ");
            }
            return Ok(result);
        }
    }
}
