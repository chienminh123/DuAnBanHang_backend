using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetailTopping_NguyenLieu_NguyenLieuId",
                table: "OrderDetailTopping");

            migrationBuilder.RenameColumn(
                name: "NguyenLieuId",
                table: "OrderDetailTopping",
                newName: "ToppingSanPhamId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetailTopping_NguyenLieuId",
                table: "OrderDetailTopping",
                newName: "IX_OrderDetailTopping_ToppingSanPhamId");

            migrationBuilder.AddColumn<int>(
                name: "RauCuNguyenLieuId",
                table: "OrderDetail",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiaChiGiaoHang",
                table: "Order",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PhiGiaoHang",
                table: "Order",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "SdtNguoiNhan",
                table: "Order",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenNguoiNhan",
                table: "Order",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$v1ky6FrsfE5FNWLl6stSlurK9VYN0b09.0FMsQNvNli/c41yUrLF6");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_RauCuNguyenLieuId",
                table: "OrderDetail",
                column: "RauCuNguyenLieuId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_NguyenLieu_RauCuNguyenLieuId",
                table: "OrderDetail",
                column: "RauCuNguyenLieuId",
                principalTable: "NguyenLieu",
                principalColumn: "NguyenLieuId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetailTopping_sanPhams_ToppingSanPhamId",
                table: "OrderDetailTopping",
                column: "ToppingSanPhamId",
                principalTable: "sanPhams",
                principalColumn: "SanPhamId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_NguyenLieu_RauCuNguyenLieuId",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetailTopping_sanPhams_ToppingSanPhamId",
                table: "OrderDetailTopping");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetail_RauCuNguyenLieuId",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "RauCuNguyenLieuId",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "DiaChiGiaoHang",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PhiGiaoHang",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "SdtNguoiNhan",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "TenNguoiNhan",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "ToppingSanPhamId",
                table: "OrderDetailTopping",
                newName: "NguyenLieuId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetailTopping_ToppingSanPhamId",
                table: "OrderDetailTopping",
                newName: "IX_OrderDetailTopping_NguyenLieuId");

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$vMSxaqPq5jemXr0rcfQnCu0nDOsbGREYrXANQ6H.K0/.YHdxgTax6");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetailTopping_NguyenLieu_NguyenLieuId",
                table: "OrderDetailTopping",
                column: "NguyenLieuId",
                principalTable: "NguyenLieu",
                principalColumn: "NguyenLieuId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
