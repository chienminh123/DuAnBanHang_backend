using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class ChamCong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BanKinhChoPhep",
                table: "cuaHangs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "cuaHangs",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "cuaHangs",
                type: "float",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CaLamViec",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenCa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GioBatDau = table.Column<TimeSpan>(type: "time", nullable: false),
                    GioKetThuc = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaLamViec", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChamCong",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaiKhanNoiBoId = table.Column<int>(type: "int", nullable: false),
                    CaLamViecId = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioVao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GioRa = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChamCong", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$RM1qaucw1q9C3qvdwKdTP.q0wVWFbejkV1pFNckpXX3rW0AUj2A1u");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaLamViec");

            migrationBuilder.DropTable(
                name: "ChamCong");

            migrationBuilder.DropColumn(
                name: "BanKinhChoPhep",
                table: "cuaHangs");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "cuaHangs");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "cuaHangs");

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$3HP5UBaEEwNN9Eq35VLvneqWlxgwPR3JUtCu4DugIrAGztJgkhTuq");
        }
    }
}
