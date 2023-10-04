using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightBoostApplication.Data.Migrations
{
    public partial class UpdateQuestionsAnswerNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "answer",
                table: "Questions",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "answer",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
