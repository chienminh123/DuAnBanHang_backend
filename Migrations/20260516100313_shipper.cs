using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class shipper : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LalamoveOrderId",
                table: "Order",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LalamoveQuotationId",
                table: "Order",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LalamoveTrackingUrl",
                table: "Order",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$qhn3mmzpP8yGBxMIdZ/DOOrzTAMxiraXZptj4lP/CSgF/uh4oN9va");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LalamoveOrderId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "LalamoveQuotationId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "LalamoveTrackingUrl",
                table: "Order");

            migrationBuilder.UpdateData(
                table: "TaiKhoanNoiBo",
                keyColumn: "TaiKhoanNoiBoId",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$T4aHr3Q.6j4t7vNrzATzpuRHhXFqS/OB8DhsYyFfIxeVl.kI1ZjT.");
        }
    }
}
