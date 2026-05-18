using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class k : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$LQW8IdjKdClXmmxWb7EIAeT5OoIk3tqyynO2290d9o1.U4CgUEn52");

            migrationBuilder.CreateIndex(
                name: "IX_ChamCong_CaLamViecId",
                table: "ChamCong",
                column: "CaLamViecId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChamCong_CaLamViec_CaLamViecId",
                table: "ChamCong",
                column: "CaLamViecId",
                principalTable: "CaLamViec",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChamCong_CaLamViec_CaLamViecId",
                table: "ChamCong");

            migrationBuilder.DropIndex(
                name: "IX_ChamCong_CaLamViecId",
                table: "ChamCong");

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$qhn3mmzpP8yGBxMIdZ/DOOrzTAMxiraXZptj4lP/CSgF/uh4oN9va");
        }
    }
}
