using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class OrderSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KhachHangId = table.Column<int>(type: "int", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cart_taiKhoanKhachHang_KhachHangId",
                        column: x => x.KhachHangId,
                        principalTable: "taiKhoanKhachHang",
                        principalColumn: "KhachHangId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDonHang = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LoaiDonHang = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KhachHangId = table.Column<int>(type: "int", nullable: true),
                    TaiKhoanNoiBoId = table.Column<int>(type: "int", nullable: true),
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    TongTienHang = table.Column<double>(type: "float", nullable: false),
                    TienGiamGia = table.Column<double>(type: "float", nullable: false),
                    DiemSuDung = table.Column<double>(type: "float", nullable: false),
                    DiemCongThem = table.Column<double>(type: "float", nullable: false),
                    ThanhTien = table.Column<double>(type: "float", nullable: false),
                    PhuongThucThanhToan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsThanhToan = table.Column<bool>(type: "bit", nullable: false),
                    TrangThaiDonHang = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_cuaHangs_ShopId",
                        column: x => x.ShopId,
                        principalTable: "cuaHangs",
                        principalColumn: "ShopId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Order_taiKhoanKhachHang_KhachHangId",
                        column: x => x.KhachHangId,
                        principalTable: "taiKhoanKhachHang",
                        principalColumn: "KhachHangId");
                    table.ForeignKey(
                        name: "FK_Order_TaiKhoanNoiBo_TaiKhoanNoiBoId",
                        column: x => x.TaiKhoanNoiBoId,
                        principalTable: "TaiKhoanNoiBo",
                        principalColumn: "TaiKhoanNoiBoId");
                });

            migrationBuilder.CreateTable(
                name: "CartDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartId = table.Column<int>(type: "int", nullable: false),
                    SanPhamId = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartDetail_Cart_CartId",
                        column: x => x.CartId,
                        principalTable: "Cart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    SanPhamId = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<double>(type: "float", nullable: false),
                    ThanhTien = table.Column<double>(type: "float", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$fgI4AAuOrfAn9PcKCQ6fturOzZitz9jc3Gw9beWmlgMXI72ottvfa");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_KhachHangId",
                table: "Cart",
                column: "KhachHangId");

            migrationBuilder.CreateIndex(
                name: "IX_CartDetail_CartId",
                table: "CartDetail",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_KhachHangId",
                table: "Order",
                column: "KhachHangId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_ShopId",
                table: "Order",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_TaiKhoanNoiBoId",
                table: "Order",
                column: "TaiKhoanNoiBoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_OrderId",
                table: "OrderDetail",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartDetail");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "Cart");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$QKfdMNDG8EiD3Xk26bepUunojUg1Ytbv8LTBcljQuqjSAN6XmgAly");
        }
    }
}
