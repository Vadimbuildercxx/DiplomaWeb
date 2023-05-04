using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diploma.Migrations
{
    public partial class AddPPE : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cameras_Areas_AreaId",
                table: "Cameras");

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Cameras_CameraId",
                table: "Logs");

            migrationBuilder.DropIndex(
                name: "IX_Logs_CameraId",
                table: "Logs");

            migrationBuilder.DropIndex(
                name: "IX_Cameras_AreaId",
                table: "Cameras");

            migrationBuilder.AddColumn<int>(
                name: "PPEId",
                table: "Logs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PPEs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPEs", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PPEs");

            migrationBuilder.DropColumn(
                name: "PPEId",
                table: "Logs");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_CameraId",
                table: "Logs",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_AreaId",
                table: "Cameras",
                column: "AreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cameras_Areas_AreaId",
                table: "Cameras",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Cameras_CameraId",
                table: "Logs",
                column: "CameraId",
                principalTable: "Cameras",
                principalColumn: "Id");
        }
    }
}
