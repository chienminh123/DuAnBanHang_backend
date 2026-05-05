using backend.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.Client
{
    [Route("api/client/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly ShopContext _context;

        public MenuController(ShopContext context)
        {
            _context = context;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetMenu()
        {
            var rawProducts = await _context.sanPhams
                                    .Include(p => p.TheLoai)
                                    .Where(p => p.IsActive)
                                    .ToListAsync();

            var groupedMenu = rawProducts
                .GroupBy(p => p.SanPhamName.Trim().ToLower())
                .Select(g => {
                    var firstItem = g.First();

                    return new
                    {
                        SanPhamName = firstItem.SanPhamName, 
                        TenTheLoai = firstItem.TheLoai?.TheLoaiName ?? "Khác",
                        HinhAnh = firstItem.HinhAnh,
                        MoTa = firstItem.MoTa,

                        GiaBanTu = g.Min(p => p.GiaBan),

                        Options = g.Select(p => new
                        {
                            SanPhamId = p.SanPhamId,
                            Size = p.Size,
                            GiaBan = p.GiaBan
                        }).OrderBy(o => o.GiaBan).ToList()
                    };
                })
                .ToList();

            return Ok(groupedMenu);
        }

        [HttpGet("extras")]
        public async Task<IActionResult> GetExtras()
        {
            //  Rau củ 
            var veggies = await _context.NguyenLieu
                .Include(n => n.TheLoai)
                .Where(n => n.IsActive && n.TheLoai != null && n.TheLoai.TheLoaiName.ToLower().Contains("rau củ"))
                .Select(n => new { id = n.NguyenLieuId, name = n.NguyenLieuName })
                .ToListAsync();

            //  Topping 
            var toppings = await _context.sanPhams
                .Include(s => s.TheLoai)
                .Where(s => s.IsActive && s.TheLoai != null && s.TheLoai.TheLoaiName.ToLower().Contains("topping"))
                .Select(s => new { id = s.SanPhamId, name = s.SanPhamName, price = s.GiaBan })
                .ToListAsync();

            return Ok(new { veggies, toppings });
        }
    }
}
