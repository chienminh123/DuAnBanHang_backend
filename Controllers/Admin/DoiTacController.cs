using backend.Data;
using backend.DTOs.Admin;
using backend.Models.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace backend.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoiTacController : ControllerBase
    {
        private readonly ShopContext _context;

        public DoiTacController(ShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.DoiTac.ToListAsync();
            return Ok(list);          
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm]DoiTacDTO request)
        {
            var dt = await _context.DoiTac.FirstOrDefaultAsync(d => d.Name == request.name.Trim());
            if(dt != null)
            {
                return BadRequest("Đối tác này đã tồn tại !");
            }
            if (CheckPhone(request.phone)==false)
            {
                return BadRequest("sdt k đúng định dạng");
            }
            var newDT = new DoiTac
            {
                Name = request.name,
                Phone = request.phone,
                Email = request.email,
                Address = request.address
            };
            _context.DoiTac.Add(newDT);
            await _context.SaveChangesAsync();
            return Ok("Tạo đối tác thành công ");
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id,[FromForm]DoiTacDTO request)
        {
            var doitac= await _context.DoiTac.FirstOrDefaultAsync(d=>d.Id==id);
            if (doitac == null)
            {
                return BadRequest("K tìm thấy đối tác này ");
            }
            if(!string.IsNullOrEmpty(request.name)) doitac.Name = request.name;
            if(!string.IsNullOrEmpty(request.phone) && CheckPhone(request.phone)==true) doitac.Phone = request.phone;
            if(!string.IsNullOrEmpty(request.address)) doitac.Address = request.address;
            if(!string.IsNullOrEmpty(request.email)) doitac.Email = request.email;
            await _context.SaveChangesAsync();
            return Ok("Cập nhật thành công");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var dt= await _context.DoiTac.FirstOrDefaultAsync(s=>s.Id==id);
            if (dt == null) return BadRequest("k tim thấy đối tác này");
            _context.DoiTac.Remove(dt);
            await _context.SaveChangesAsync();
            return Ok("xoa thành công ");
        }

        private bool CheckPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return false;

            string pattern = @"^(0)(3|5|7|8|9)[0-9]{8}$";
            return Regex.IsMatch(phone, pattern);
        }

        [HttpGet("search_by_id")]
        public async Task<IActionResult> SearchById([FromQuery] int id)
        {
            var dt= await _context.DoiTac.FirstOrDefaultAsync(s=>s.Id == id);   
            if (dt == null) return BadRequest("k tìm thấy đối tác này ");
            return Ok(dt);
        }

        [HttpGet("search_by_phone")] 
        public async Task<IActionResult> SearchByPhone([FromQuery] string key)
        {
            var dt = await _context.DoiTac.Where(s=>s.Phone.Contains(key)).ToListAsync();
            if (dt == null) return BadRequest("k tìm thấy đối tác này ");
            return Ok(dt);
        }

        [HttpGet("search_by_name")]
        public async Task<IActionResult> SearchByName([FromQuery] string key)
        {
            var dt = await _context.DoiTac.Where(s => s.Name.Contains(key)).ToListAsync();
            if (dt == null) return BadRequest("k tìm thấy đối tác này ");
            return Ok(dt);
        }

        [HttpGet("search_by_address")]
        public async Task<IActionResult> SearchByAddress([FromQuery] string key)
        {
            var dt = await _context.DoiTac.Where(s => s.Address.Contains(key)).ToListAsync();
            if (dt == null) return BadRequest("k tìm thấy đối tác này ");
            return Ok(dt);
        }
    }
}
