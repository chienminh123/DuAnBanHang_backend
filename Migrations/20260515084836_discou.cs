using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class discou : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiscountValue",
                table: "MaGiamGia",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$T4aHr3Q.6j4t7vNrzATzpuRHhXFqS/OB8DhsYyFfIxeVl.kI1ZjT.");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_SanPhamId",
                table: "OrderDetail",
                column: "SanPhamId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_sanPhams_SanPhamId",
                table: "OrderDetail",
                column: "SanPhamId",
                principalTable: "sanPhams",
                principalColumn: "SanPhamId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_sanPhams_SanPhamId",
                table: "OrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetail_SanPhamId",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "DiscountValue",
                table: "MaGiamGia");

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$rasO/QibE2jRbB.brI2PjOMGWAH9e0FYt.ddgVMopBwLcG0G20iXS");
        }
    }
}
