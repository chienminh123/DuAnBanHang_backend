using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class BangLuong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BangLuong",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaiKhoanNoiBoId = table.Column<int>(type: "int", nullable: false),
                    Thang = table.Column<int>(type: "int", nullable: false),
                    Nam = table.Column<int>(type: "int", nullable: false),
                    TongGioLam = table.Column<double>(type: "float", nullable: false),
                    TongNgayLam = table.Column<int>(type: "int", nullable: false),
                    TienPhatDiMuon = table.Column<double>(type: "float", nullable: false),
                    TongTienNhan = table.Column<double>(type: "float", nullable: false),
                    TongPhuCap = table.Column<double>(type: "float", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BangLuong", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BangLuong_TaiKhoanNoiBo_TaiKhoanNoiBoId",
                        column: x => x.TaiKhoanNoiBoId,
                        principalTable: "TaiKhoanNoiBo",
                        principalColumn: "TaiKhoanNoiBoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CauHinhLuong",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaiKhoanNoiBoId = table.Column<int>(type: "int", nullable: false),
                    LoaiLuong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MucLuong = table.Column<double>(type: "float", nullable: false),
                    PhuCap = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHinhLuong", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CauHinhLuong_TaiKhoanNoiBo_TaiKhoanNoiBoId",
                        column: x => x.TaiKhoanNoiBoId,
                        principalTable: "TaiKhoanNoiBo",
                        principalColumn: "TaiKhoanNoiBoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$sK4bfQrcFJNXnLrWt6G26e0sO/0JOHRgh8NTSKS6OMtyYG3mwQxri");

            migrationBuilder.CreateIndex(
                name: "IX_BangLuong_TaiKhoanNoiBoId",
                table: "BangLuong",
                column: "TaiKhoanNoiBoId");

            migrationBuilder.CreateIndex(
                name: "IX_CauHinhLuong_TaiKhoanNoiBoId",
                table: "CauHinhLuong",
                column: "TaiKhoanNoiBoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BangLuong");

            migrationBuilder.DropTable(
                name: "CauHinhLuong");

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$QH2MaHbecYSnJHS9XGTw/OTbXZWb8dJ43ShEZ1mzFn0.kf.WiYJ7u");
        }
    }
}
