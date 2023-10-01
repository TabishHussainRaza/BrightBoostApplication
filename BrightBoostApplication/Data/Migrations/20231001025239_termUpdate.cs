using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightBoostApplication.Data.Migrations
{
    public partial class termUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "Terms",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "Terms");
        }
    }
}
