using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightBoostApplication.Data.Migrations
{
    public partial class tutor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fkId",
                table: "Tutors");

            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "Tutors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userId",
                table: "Tutors");

            migrationBuilder.AddColumn<int>(
                name: "fkId",
                table: "Tutors",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
