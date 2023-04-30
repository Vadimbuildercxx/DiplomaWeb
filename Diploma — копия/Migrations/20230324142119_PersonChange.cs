using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diploma.Migrations
{
    public partial class PersonChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMale",
                table: "Persons");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Persons",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Persons");

            migrationBuilder.AddColumn<bool>(
                name: "IsMale",
                table: "Persons",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
