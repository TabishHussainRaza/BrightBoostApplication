using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightBoostApplication.Data.Migrations
{
    public partial class tutorUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Availability",
                table: "Tutors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Availability",
                table: "Tutors");
        }
    }
}
