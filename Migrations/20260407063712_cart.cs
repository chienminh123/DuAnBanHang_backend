using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class cart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsKhongNau",
                table: "OrderDetail",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsKhongNau",
                table: "CartDetail",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RauCuNguyenLieuId",
                table: "CartDetail",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CartDetailTopping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartDetailId = table.Column<int>(type: "int", nullable: false),
                    ToppingSanPhamId = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartDetailTopping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartDetailTopping_CartDetail_CartDetailId",
                        column: x => x.CartDetailId,
                        principalTable: "CartDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartDetailTopping_sanPhams_ToppingSanPhamId",
                        column: x => x.ToppingSanPhamId,
                        principalTable: "sanPhams",
                        principalColumn: "SanPhamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetailTopping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDetailId = table.Column<int>(type: "int", nullable: false),
                    NguyenLieuId = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    GiaTopping = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetailTopping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetailTopping_NguyenLieu_NguyenLieuId",
                        column: x => x.NguyenLieuId,
                        principalTable: "NguyenLieu",
                        principalColumn: "NguyenLieuId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetailTopping_OrderDetail_OrderDetailId",
                        column: x => x.OrderDetailId,
                        principalTable: "OrderDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$vMSxaqPq5jemXr0rcfQnCu0nDOsbGREYrXANQ6H.K0/.YHdxgTax6");

            migrationBuilder.CreateIndex(
                name: "IX_CartDetail_RauCuNguyenLieuId",
                table: "CartDetail",
                column: "RauCuNguyenLieuId");

            migrationBuilder.CreateIndex(
                name: "IX_CartDetailTopping_CartDetailId",
                table: "CartDetailTopping",
                column: "CartDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_CartDetailTopping_ToppingSanPhamId",
                table: "CartDetailTopping",
                column: "ToppingSanPhamId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetailTopping_NguyenLieuId",
                table: "OrderDetailTopping",
                column: "NguyenLieuId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetailTopping_OrderDetailId",
                table: "OrderDetailTopping",
                column: "OrderDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetail_NguyenLieu_RauCuNguyenLieuId",
                table: "CartDetail",
                column: "RauCuNguyenLieuId",
                principalTable: "NguyenLieu",
                principalColumn: "NguyenLieuId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetail_NguyenLieu_RauCuNguyenLieuId",
                table: "CartDetail");

            migrationBuilder.DropTable(
                name: "CartDetailTopping");

            migrationBuilder.DropTable(
                name: "OrderDetailTopping");

            migrationBuilder.DropIndex(
                name: "IX_CartDetail_RauCuNguyenLieuId",
                table: "CartDetail");

            migrationBuilder.DropColumn(
                name: "IsKhongNau",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "IsKhongNau",
                table: "CartDetail");

            migrationBuilder.DropColumn(
                name: "RauCuNguyenLieuId",
                table: "CartDetail");

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$fgI4AAuOrfAn9PcKCQ6fturOzZitz9jc3Gw9beWmlgMXI72ottvfa");
        }
    }
}
