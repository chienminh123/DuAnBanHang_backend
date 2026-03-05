using backend.DTOs.Admin;
using backend.Models.Admin;
using backend.Models.Client;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class ShopContext:DbContext
    {
        public ShopContext(DbContextOptions<ShopContext> options) : base(options) { }

        public DbSet<TaiKhoanNoiBo> TaiKhoanNoiBo { get; set; }
        public DbSet<ChucVu> ChucVu { get; set; }
        public DbSet<TaiKhoanKhachHang> taiKhoanKhachHang { get; set; }
        public DbSet<CuaHang> cuaHangs { get; set; }
        public DbSet<TheLoai> theLoais { get; set; }
        public DbSet<SanPham> sanPhams { get; set; }
        public DbSet<BienLai> BienLai { get; set; }
        public DbSet<ChiTietBienLai> ChiTietBienLai { get; set; }
        public DbSet<NguyenLieu> NguyenLieu  { get; set; }
        public DbSet<TonKho> tonKhos { get; set; }
        public DbSet<DoiTac> DoiTac { get; set; }
        public DbSet<MaGiamGia> MaGiamGia { get; set; }
        public DbSet<KiemKe> KiemKe { get; set; }
        public DbSet<ChiTietKiemKe> ChiTietKiemKe { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<NguyenLieu>()
                    .HasOne(n => n.TheLoai)
                    .WithMany()
                    .HasForeignKey(n => n.TheLoaiId)
                    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChucVu>().HasData(
                new ChucVu { ChucVuId = 1, ChucVuName = "Giám đốc", ChucVuDescription="Người quản lý tất cả. tạo tài khoản các phòng ban" },
                new ChucVu { ChucVuId = 2, ChucVuName = "Kế toán", ChucVuDescription="Người tính lương, duyệt thu chi từ kho và QL khu vực"},
                new ChucVu { ChucVuId = 3, ChucVuName = "Sản xuất", ChucVuDescription = "Người tiếp nhận đặt hàng từ cửa hàng, đặt hàng với bên thứ 3, phiếu nhập và xuất kho từ CH và bên thứ 3" },
                new ChucVu { ChucVuId = 4, ChucVuName = "Quản lý khu vực", ChucVuDescription = "Người quản lý các CH do GD phân, làm phiếu đề xuất khi CH cần, ql công ca NV cửa hàng" },
                new ChucVu { ChucVuId = 5, ChucVuName = "Quản lý cửa hàng", ChucVuDescription = "Người đặt thực phẩm với kho, tạo các phiếu điều chuyển giữ các cửa hàng, xem báo cáo doanh thu và đặt thực phẩm" },
                new ChucVu { ChucVuId = 6, ChucVuName = "Nhân viên bán hàng", ChucVuDescription = "xem được công ca, phiếu lương" },
                new ChucVu { ChucVuId = 7, ChucVuName = "Khách hàng", ChucVuDescription = "Đặt hàng qua web, tích điểm" },
                new ChucVu { ChucVuId = 8, ChucVuName = "Máy Pos", ChucVuDescription = "máy ở CH để nhân viên order trực tiếp" },
                new ChucVu { ChucVuId = 9, ChucVuName = "Nhân sự", ChucVuDescription = "Phân chức vụ các phòng ban " }
            );

            modelBuilder.Entity<TaiKhoanNoiBo>().HasData(
                new TaiKhoanNoiBo
                {
                    TaiKhoanNoiBoId = 1,
                    TenTaiKhoan = "nmchien@giamdoc.food.com",
                    MatKhau = BCrypt.Net.BCrypt.HashPassword("Chien@12345"),
                    Avatar="",
                    TenNhanVien = "Nguyễn Minh Chiến",
                    Email = "cn908820@gmail.com",
                    Sdt = "0909999999",
                    NgaySinh = new DateTime(2004, 4, 8),
                    NgayThamGia = new DateTime(2025, 1, 1),
                    IsActive = true,
                    ChucVuId = 1 ,
                    CuaHangId=null
                }
            );

            modelBuilder.Entity<TheLoai>().HasData(
               new TheLoai{ TheLoaiId=1, TheLoaiName="Bán thành phẩm"},
               new TheLoai { TheLoaiId = 2, TheLoaiName = "Thành phẩm" },
               new TheLoai { TheLoaiId = 3, TheLoaiName = "Rau củ " },
               new TheLoai { TheLoaiId = 4, TheLoaiName = "Công cụ dụng cụ " },
               new TheLoai { TheLoaiId = 5, TheLoaiName = "Nguyên vật liệu " },
               new TheLoai { TheLoaiId = 6, TheLoaiName = "Đồ chơi " },
               new TheLoai { TheLoaiId = 7, TheLoaiName = "Thương mại " },
               new TheLoai { TheLoaiId = 8, TheLoaiName = "Topping" },
               new TheLoai { TheLoaiId = 9, TheLoaiName = "Sữa hạt" }
           );
        }
    }
}
