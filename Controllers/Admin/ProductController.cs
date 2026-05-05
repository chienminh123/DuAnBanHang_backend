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
    public class ProductController : ControllerBase
    {
        private readonly ShopContext _context;
        private readonly IWebHostEnvironment _env; 

        public ProductController(ShopContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list= await _context.sanPhams.Include(s=>s.TheLoai).ToListAsync();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SanPhamDTO request)
        {
            string urlHinhAnh = "";
            if(request.HinhAnhUpload != null)
            {
                var tenFile = Guid.NewGuid().ToString() + Path.GetExtension(request.HinhAnhUpload.FileName);
                var path = Path.Combine(_env.WebRootPath, "images/sanpham", tenFile);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await request.HinhAnhUpload.CopyToAsync(stream);
                }
                urlHinhAnh = "/images/sanpham/" + tenFile;
            }

            var sanPhamMoi = new SanPham
            {
                SanPhamName=request.SanPhamName,
                GiaBan=request.GiaBan,
                MoTa=request.MoTa,
                HinhAnh=urlHinhAnh,
                Size=request.Size,
                TheLoaiId=request.TheLoaiId,
                NguyenLieuId=request.NguyenLieuId,
                IsActive=true
            };
            _context.sanPhams.Add(sanPhamMoi);
            await _context.SaveChangesAsync();
            return Ok(sanPhamMoi);
        }

        [HttpDelete("id")]
        public async Task<IActionResult> Delete(int id)
        {
            var sp= await _context.sanPhams.FindAsync(id);
            if (sp == null)
            {
                return NotFound("Không tìm thấy sản phẩm!");
            }
            if (!string.IsNullOrEmpty(sp.HinhAnh))
            {
                var oldPath=Path.Combine(_env.WebRootPath, sp.HinhAnh.TrimStart('/'));
                if (System.IO.File.Exists(oldPath)) 
                { 
                    System.IO.File.Delete(oldPath);
                }
            }
            _context.sanPhams.Remove(sp);
            await _context.SaveChangesAsync();
            return Ok($"Đã xóa sản phẩm mã {id}");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] SanPhamDTO request)
        {
            var sp = await _context.sanPhams.FindAsync(id);

            if (sp == null)
            {
                return NotFound("Không tìm thấy món ăn cần sửa.");
            }

            sp.SanPhamName = request.SanPhamName;
            sp.TheLoaiId = request.TheLoaiId;
            sp.Size = request.Size;
            sp.MoTa = request.MoTa;
            sp.GiaBan = request.GiaBan;
            sp.IsActive = request.IsActive;

            if (request.HinhAnhUpload != null)
            {
                if (!string.IsNullOrEmpty(sp.HinhAnh))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, sp.HinhAnh.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }
                var tenFile = Guid.NewGuid().ToString() + Path.GetExtension(request.HinhAnhUpload.FileName);
                var path = Path.Combine(_env.WebRootPath, "images","sanpham", tenFile);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await request.HinhAnhUpload.CopyToAsync(stream);
                }
                sp.HinhAnh = "/images/sanpham/" + tenFile;
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thành công!", data = sp });
        }

        [HttpGet("search-by-name")]
        public async Task<IActionResult> SearchByName([FromQuery] string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return await GetAll();
            }

            var result = await _context.sanPhams
                .Include(s => s.TheLoai) 
                .Where(s => s.SanPhamName.Contains(keyword))
                .ToListAsync();

            if (result.Count == 0)
            {
                return NotFound("Không tìm thấy món nào khớp với từ khóa.");
            }
            return Ok(result);
        }

        [HttpGet("search-by-id")]
        public async Task<IActionResult> SearchByID([FromQuery] int id)
        {

            var result = await _context.sanPhams.FindAsync(id);

            if (result==null)
            {
                return NotFound($"Không tìm thấy món nào có mã sản phẩm là {id}.");
            }
            return Ok(result);
        }

        [HttpGet("search-by-genre")]
        public async Task<IActionResult> SearchByGerne ([FromQuery] string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return await GetAll();
            }

            var result = await _context.sanPhams
                .Include(s => s.TheLoai)
                .Where(s => s.TheLoai.TheLoaiName.Contains(keyword))
                .ToListAsync();

            if (result.Count == 0)
            {
                return NotFound("Không tìm thấy thể loại nào khớp với từ khóa.");
            }
            return Ok(result);
        }
    }
}
