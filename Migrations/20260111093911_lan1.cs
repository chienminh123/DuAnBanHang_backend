using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class lan1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChucVu",
                columns: table => new
                {
                    ChucVuId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChucVuName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ChucVuDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChucVu", x => x.ChucVuId);
                });

            migrationBuilder.CreateTable(
                name: "cuaHangs",
                columns: table => new
                {
                    ShopId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShopAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShopPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShopCity = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cuaHangs", x => x.ShopId);
                });

            migrationBuilder.CreateTable(
                name: "DoiTac",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoiTac", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaGiamGia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiMa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxValue = table.Column<float>(type: "real", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaGiamGia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "theLoais",
                columns: table => new
                {
                    TheLoaiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TheLoaiName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_theLoais", x => x.TheLoaiId);
                });

            migrationBuilder.CreateTable(
                name: "taiKhoanKhachHang",
                columns: table => new
                {
                    KhachHangId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenKhachHang = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Sdt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChucVuId = table.Column<int>(type: "int", nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayThamGia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TichDiem = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_taiKhoanKhachHang", x => x.KhachHangId);
                    table.ForeignKey(
                        name: "FK_taiKhoanKhachHang_ChucVu_ChucVuId",
                        column: x => x.ChucVuId,
                        principalTable: "ChucVu",
                        principalColumn: "ChucVuId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KiemKe",
                columns: table => new
                {
                    KiemKeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    NgayThucHien = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KiemKe", x => x.KiemKeId);
                    table.ForeignKey(
                        name: "FK_KiemKe_cuaHangs_ShopId",
                        column: x => x.ShopId,
                        principalTable: "cuaHangs",
                        principalColumn: "ShopId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoanNoiBo",
                columns: table => new
                {
                    TaiKhoanNoiBoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTaiKhoan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChucVuId = table.Column<int>(type: "int", nullable: false),
                    CuaHangId = table.Column<int>(type: "int", nullable: true),
                    TenNhanVien = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GioiTinh = table.Column<bool>(type: "bit", nullable: false),
                    Sdt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayThamGia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoanNoiBo", x => x.TaiKhoanNoiBoId);
                    table.ForeignKey(
                        name: "FK_TaiKhoanNoiBo_ChucVu_ChucVuId",
                        column: x => x.ChucVuId,
                        principalTable: "ChucVu",
                        principalColumn: "ChucVuId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaiKhoanNoiBo_cuaHangs_CuaHangId",
                        column: x => x.CuaHangId,
                        principalTable: "cuaHangs",
                        principalColumn: "ShopId");
                });

            migrationBuilder.CreateTable(
                name: "BienLai",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HanhDong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CuaHangId = table.Column<int>(type: "int", nullable: false),
                    KhoXuatId = table.Column<int>(type: "int", nullable: true),
                    DoiTacId = table.Column<int>(type: "int", nullable: true),
                    NgayThucHien = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BienLai", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BienLai_cuaHangs_CuaHangId",
                        column: x => x.CuaHangId,
                        principalTable: "cuaHangs",
                        principalColumn: "ShopId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BienLai_DoiTac_DoiTacId",
                        column: x => x.DoiTacId,
                        principalTable: "DoiTac",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NguyenLieu",
                columns: table => new
                {
                    NguyenLieuId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoiTacId = table.Column<int>(type: "int", nullable: true),
                    NguyenLieuName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DonVi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GiaNhap = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TheLoaiId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguyenLieu", x => x.NguyenLieuId);
                    table.ForeignKey(
                        name: "FK_NguyenLieu_DoiTac_Id",
                        column: x => x.Id,
                        principalTable: "DoiTac",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NguyenLieu_theLoais_TheLoaiId",
                        column: x => x.TheLoaiId,
                        principalTable: "theLoais",
                        principalColumn: "TheLoaiId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietBienLai",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BienLaiId = table.Column<int>(type: "int", nullable: false),
                    NguyenLieuId = table.Column<int>(type: "int", nullable: false),
                    Soluong = table.Column<float>(type: "real", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietBienLai", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietBienLai_BienLai_BienLaiId",
                        column: x => x.BienLaiId,
                        principalTable: "BienLai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietBienLai_NguyenLieu_NguyenLieuId",
                        column: x => x.NguyenLieuId,
                        principalTable: "NguyenLieu",
                        principalColumn: "NguyenLieuId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietKiemKe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KiemKeId = table.Column<int>(type: "int", nullable: false),
                    NguyenLieuId = table.Column<int>(type: "int", nullable: false),
                    TonHeThong = table.Column<float>(type: "real", nullable: false),
                    TonThucTe = table.Column<float>(type: "real", nullable: false),
                    ChenhLech = table.Column<float>(type: "real", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietKiemKe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietKiemKe_KiemKe_KiemKeId",
                        column: x => x.KiemKeId,
                        principalTable: "KiemKe",
                        principalColumn: "KiemKeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietKiemKe_NguyenLieu_NguyenLieuId",
                        column: x => x.NguyenLieuId,
                        principalTable: "NguyenLieu",
                        principalColumn: "NguyenLieuId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sanPhams",
                columns: table => new
                {
                    SanPhamId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SanPhamName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TheLoaiId = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GiaBan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NguyenLieuId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sanPhams", x => x.SanPhamId);
                    table.ForeignKey(
                        name: "FK_sanPhams_NguyenLieu_NguyenLieuId",
                        column: x => x.NguyenLieuId,
                        principalTable: "NguyenLieu",
                        principalColumn: "NguyenLieuId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sanPhams_theLoais_TheLoaiId",
                        column: x => x.TheLoaiId,
                        principalTable: "theLoais",
                        principalColumn: "TheLoaiId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tonKhos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    NguyenLieuId = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tonKhos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tonKhos_cuaHangs_ShopId",
                        column: x => x.ShopId,
                        principalTable: "cuaHangs",
                        principalColumn: "ShopId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tonKhos_NguyenLieu_NguyenLieuId",
                        column: x => x.NguyenLieuId,
                        principalTable: "NguyenLieu",
                        principalColumn: "NguyenLieuId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ChucVu",
                columns: new[] { "ChucVuId", "ChucVuDescription", "ChucVuName" },
                values: new object[,]
                {
                    { 1, "Người quản lý tất cả. tạo tài khoản các phòng ban", "Giám đốc" },
                    { 2, "Người tính lương, duyệt thu chi từ kho và QL khu vực", "Kế toán" },
                    { 3, "Người tiếp nhận đặt hàng từ cửa hàng, đặt hàng với bên thứ 3, phiếu nhập và xuất kho từ CH và bên thứ 3", "Sản xuất" },
                    { 4, "Người quản lý các CH do GD phân, làm phiếu đề xuất khi CH cần, ql công ca NV cửa hàng", "Quản lý khu vực" },
                    { 5, "Người đặt thực phẩm với kho, tạo các phiếu điều chuyển giữ các cửa hàng, xem báo cáo doanh thu và đặt thực phẩm", "Quản lý cửa hàng" },
                    { 6, "xem được công ca, phiếu lương", "Nhân viên bán hàng" },
                    { 7, "Đặt hàng qua web, tích điểm", "Khách hàng" },
                    { 8, "máy ở CH để nhân viên order trực tiếp", "Máy Pos" },
                    { 9, "Phân chức vụ các phòng ban ", "Nhân sự" }
                });

            migrationBuilder.InsertData(
                table: "theLoais",
                columns: new[] { "TheLoaiId", "TheLoaiName" },
                values: new object[,]
                {
                    { 1, "Bán thành phẩm" },
                    { 2, "Thành phẩm" },
                    { 3, "Rau củ " },
                    { 4, "Công cụ dụng cụ " },
                    { 5, "Nguyên vật liệu " },
                    { 6, "Đồ chơi " },
                    { 7, "Thương mại " },
                    { 8, "Topping" },
                    { 9, "Sữa hạt" }
                });

            migrationBuilder.InsertData(
                table: "TaiKhoanNoiBo",
                columns: new[] { "TaiKhoanNoiBoId", "Avatar", "ChucVuId", "CuaHangId", "Email", "GioiTinh", "IsActive", "MatKhau", "NgaySinh", "NgayThamGia", "Sdt", "TenNhanVien", "TenTaiKhoan" },
                values: new object[] { 1, "", 1, null, "cn908820@gmail.com", false, true, "$2a$11$k4wlmXvDdX2rlleAgDk9BOI66WTWUmunNfvSkSZxeK3uY3/j/0.bm", new DateTime(2004, 4, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "0909999999", "Nguyễn Minh Chiến", "nmchien@food.com" });

            migrationBuilder.CreateIndex(
                name: "IX_BienLai_CuaHangId",
                table: "BienLai",
                column: "CuaHangId");

            migrationBuilder.CreateIndex(
                name: "IX_BienLai_DoiTacId",
                table: "BienLai",
                column: "DoiTacId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietBienLai_BienLaiId",
                table: "ChiTietBienLai",
                column: "BienLaiId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietBienLai_NguyenLieuId",
                table: "ChiTietBienLai",
                column: "NguyenLieuId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietKiemKe_KiemKeId",
                table: "ChiTietKiemKe",
                column: "KiemKeId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietKiemKe_NguyenLieuId",
                table: "ChiTietKiemKe",
                column: "NguyenLieuId");

            migrationBuilder.CreateIndex(
                name: "IX_KiemKe_ShopId",
                table: "KiemKe",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_NguyenLieu_Id",
                table: "NguyenLieu",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_NguyenLieu_TheLoaiId",
                table: "NguyenLieu",
                column: "TheLoaiId");

            migrationBuilder.CreateIndex(
                name: "IX_sanPhams_NguyenLieuId",
                table: "sanPhams",
                column: "NguyenLieuId");

            migrationBuilder.CreateIndex(
                name: "IX_sanPhams_TheLoaiId",
                table: "sanPhams",
                column: "TheLoaiId");

            migrationBuilder.CreateIndex(
                name: "IX_taiKhoanKhachHang_ChucVuId",
                table: "taiKhoanKhachHang",
                column: "ChucVuId");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoanNoiBo_ChucVuId",
                table: "TaiKhoanNoiBo",
                column: "ChucVuId");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoanNoiBo_CuaHangId",
                table: "TaiKhoanNoiBo",
                column: "CuaHangId");

            migrationBuilder.CreateIndex(
                name: "IX_tonKhos_NguyenLieuId",
                table: "tonKhos",
                column: "NguyenLieuId");

            migrationBuilder.CreateIndex(
                name: "IX_tonKhos_ShopId",
                table: "tonKhos",
                column: "ShopId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietBienLai");

            migrationBuilder.DropTable(
                name: "ChiTietKiemKe");

            migrationBuilder.DropTable(
                name: "MaGiamGia");

            migrationBuilder.DropTable(
                name: "sanPhams");

            migrationBuilder.DropTable(
                name: "taiKhoanKhachHang");

            migrationBuilder.DropTable(
                name: "TaiKhoanNoiBo");

            migrationBuilder.DropTable(
                name: "tonKhos");

            migrationBuilder.DropTable(
                name: "BienLai");

            migrationBuilder.DropTable(
                name: "KiemKe");

            migrationBuilder.DropTable(
                name: "ChucVu");

            migrationBuilder.DropTable(
                name: "NguyenLieu");

            migrationBuilder.DropTable(
                name: "cuaHangs");

            migrationBuilder.DropTable(
                name: "DoiTac");

            migrationBuilder.DropTable(
                name: "theLoais");
        }
    }
}
