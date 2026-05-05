using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class ShopMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuCuaHang",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    SanPhamId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuCuaHang", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuCuaHang_cuaHangs_ShopId",
                        column: x => x.ShopId,
                        principalTable: "cuaHangs",
                        principalColumn: "ShopId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuCuaHang_sanPhams_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "sanPhams",
                        principalColumn: "SanPhamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$2BuHENwalGt2xGAiAqtwYuapmAxEj/pwJ2NJofiJ3dp/rHaINK06.");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCuaHang_SanPhamId",
                table: "MenuCuaHang",
                column: "SanPhamId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCuaHang_ShopId",
                table: "MenuCuaHang",
                column: "ShopId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuCuaHang");

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$v1ky6FrsfE5FNWLl6stSlurK9VYN0b09.0FMsQNvNli/c41yUrLF6");
        }
    }
}
