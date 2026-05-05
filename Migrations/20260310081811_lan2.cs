using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class lan2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NguyenLieu_DoiTac_Id",
                table: "NguyenLieu");

            migrationBuilder.DropIndex(
                name: "IX_NguyenLieu_Id",
                table: "NguyenLieu");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "NguyenLieu");

            migrationBuilder.UpdateData(
                table: "ChucVu",
                keyColumn: "ChucVuId",
                keyValue: 1,
                column: "ChucVuName",
                value: "Admin");

            migrationBuilder.UpdateData(
                table: "ChucVu",
                keyColumn: "ChucVuId",
                keyValue: 2,
                column: "ChucVuName",
                value: "Accountant");

            migrationBuilder.UpdateData(
                table: "ChucVu",
                keyColumn: "ChucVuId",
                keyValue: 3,
                column: "ChucVuName",
                value: "Production");

            migrationBuilder.UpdateData(
                table: "ChucVu",
                keyColumn: "ChucVuId",
                keyValue: 4,
                column: "ChucVuName",
                value: "AreaManager");

            migrationBuilder.UpdateData(
                table: "ChucVu",
                keyColumn: "ChucVuId",
                keyValue: 5,
                column: "ChucVuName",
                value: "StoreManager");

            migrationBuilder.UpdateData(
                table: "ChucVu",
                keyColumn: "ChucVuId",
                keyValue: 6,
                column: "ChucVuName",
                value: "Sales");

            migrationBuilder.UpdateData(
                table: "ChucVu",
                keyColumn: "ChucVuId",
                keyValue: 7,
                column: "ChucVuName",
                value: "Customer");

            migrationBuilder.UpdateData(
                table: "ChucVu",
                keyColumn: "ChucVuId",
                keyValue: 8,
                column: "ChucVuName",
                value: "POS");

            migrationBuilder.UpdateData(
                table: "ChucVu",
                keyColumn: "ChucVuId",
                keyValue: 9,
                column: "ChucVuName",
                value: "HR");

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                columns: new[] { "MatKhau", "TenTaiKhoan" },
                values: new object[] { "$2a$11$ylQSNeB.1LMmNXcsUXoXbeVUthF.Dh8y7l0pahPro6wqk4BO4mBKe", "nmchien@giamdoc.food.com" });

            migrationBuilder.CreateIndex(
                name: "IX_NguyenLieu_DoiTacId",
                table: "NguyenLieu",
                column: "DoiTacId");

            migrationBuilder.AddForeignKey(
                name: "FK_NguyenLieu_DoiTac_DoiTacId",
                table: "NguyenLieu",
                column: "DoiTacId",
                principalTable: "DoiTac",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NguyenLieu_DoiTac_DoiTacId",
                table: "NguyenLieu");

            migrationBuilder.DropIndex(
                name: "IX_NguyenLieu_DoiTacId",
                table: "NguyenLieu");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "NguyenLieu",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ChucVu",
                keyColumn: "ChucVuId",
                keyValue: 1,
                column: "ChucVuName",
                value: "Giám đốc");

            migrationBuilder.UpdateData(
                table: "ChucVu",
                keyColumn: "ChucVuId",
                keyValue: 2,
                column: "ChucVuName",
                value: "Kế toán");

            migrationBuilder.UpdateData(
                table: "ChucVu",
                keyColumn: "ChucVuId",
                keyValue: 3,
                column: "ChucVuName",
                value: "Sản xuất");

            migrationBuilder.UpdateData(
                table: "ChucVu",
                keyColumn: "ChucVuId",
                keyValue: 4,
                column: "ChucVuName",
                value: "Quản lý khu vực");

            migrationBuilder.UpdateData(
                table: "ChucVu",
                keyColumn: "ChucVuId",
                keyValue: 5,
                column: "ChucVuName",
                value: "Quản lý cửa hàng");

            migrationBuilder.UpdateData(
                table: "ChucVu",
                keyColumn: "ChucVuId",
                keyValue: 6,
                column: "ChucVuName",
                value: "Nhân viên bán hàng");

            migrationBuilder.UpdateData(
                table: "ChucVu",
                keyColumn: "ChucVuId",
                keyValue: 7,
                column: "ChucVuName",
                value: "Khách hàng");

            migrationBuilder.UpdateData(
                table: "ChucVu",
                keyColumn: "ChucVuId",
                keyValue: 8,
                column: "ChucVuName",
                value: "Máy Pos");

            migrationBuilder.UpdateData(
                table: "ChucVu",
                keyColumn: "ChucVuId",
                keyValue: 9,
                column: "ChucVuName",
                value: "Nhân sự");

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                columns: new[] { "MatKhau", "TenTaiKhoan" },
                values: new object[] { "$2a$11$k4wlmXvDdX2rlleAgDk9BOI66WTWUmunNfvSkSZxeK3uY3/j/0.bm", "nmchien@food.com" });

            migrationBuilder.CreateIndex(
                name: "IX_NguyenLieu_Id",
                table: "NguyenLieu",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NguyenLieu_DoiTac_Id",
                table: "NguyenLieu",
                column: "Id",
                principalTable: "DoiTac",
                principalColumn: "Id");
        }
    }
}
