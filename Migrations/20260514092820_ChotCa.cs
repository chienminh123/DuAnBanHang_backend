using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class ChotCa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChotCa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    ThoiGianMo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianDong = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TienDauCa = table.Column<double>(type: "float", nullable: true),
                    TongThuTienMat = table.Column<double>(type: "float", nullable: false),
                    TongThuVNPAY = table.Column<double>(type: "float", nullable: false),
                    TienMatThucTe = table.Column<double>(type: "float", nullable: true),
                    VnpayThucTe = table.Column<double>(type: "float", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChotCa", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$rasO/QibE2jRbB.brI2PjOMGWAH9e0FYt.ddgVMopBwLcG0G20iXS");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChotCa");

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$2BuHENwalGt2xGAiAqtwYuapmAxEj/pwJ2NJofiJ3dp/rHaINK06.");
        }
    }
}
