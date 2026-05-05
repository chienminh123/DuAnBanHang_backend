using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class PhuCap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhuCap",
                table: "CauHinhLuong",
                newName: "PhuCapXangXe");

            migrationBuilder.AddColumn<double>(
                name: "PhuCapAnTrua",
                table: "CauHinhLuong",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PhuCapChuyenCan",
                table: "CauHinhLuong",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<double>(
                name: "TongNgayLam",
                table: "BangLuong",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<double>(
                name: "PhuCapAnTrua",
                table: "BangLuong",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PhuCapChuyenCan",
                table: "BangLuong",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PhuCapXangXe",
                table: "BangLuong",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$QKfdMNDG8EiD3Xk26bepUunojUg1Ytbv8LTBcljQuqjSAN6XmgAly");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhuCapAnTrua",
                table: "CauHinhLuong");

            migrationBuilder.DropColumn(
                name: "PhuCapChuyenCan",
                table: "CauHinhLuong");

            migrationBuilder.DropColumn(
                name: "PhuCapAnTrua",
                table: "BangLuong");

            migrationBuilder.DropColumn(
                name: "PhuCapChuyenCan",
                table: "BangLuong");

            migrationBuilder.DropColumn(
                name: "PhuCapXangXe",
                table: "BangLuong");

            migrationBuilder.RenameColumn(
                name: "PhuCapXangXe",
                table: "CauHinhLuong",
                newName: "PhuCap");

            migrationBuilder.AlterColumn<int>(
                name: "TongNgayLam",
                table: "BangLuong",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$sK4bfQrcFJNXnLrWt6G26e0sO/0JOHRgh8NTSKS6OMtyYG3mwQxri");
        }
    }
}
