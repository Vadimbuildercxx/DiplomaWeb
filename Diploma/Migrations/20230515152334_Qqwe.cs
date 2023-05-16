using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diploma.Migrations
{
    public partial class Qqwe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PPEId",
                table: "Logs");

            migrationBuilder.AddColumn<string>(
                name: "PPEitem",
                table: "Logs",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PPEitem",
                table: "Logs");

            migrationBuilder.AddColumn<int>(
                name: "PPEId",
                table: "Logs",
                type: "int",
                nullable: true);
        }
    }
}
