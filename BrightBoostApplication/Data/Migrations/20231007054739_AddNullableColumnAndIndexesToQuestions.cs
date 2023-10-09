using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightBoostApplication.Data.Migrations
{
    public partial class AddNullableColumnAndIndexesToQuestions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TutorAllocationId",
                table: "Questions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_TutorAllocationId",
                table: "Questions",
                column: "TutorAllocationId");

            migrationBuilder.AlterColumn<int>(
                name: "StudentSignUpId",
                table: "Questions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_StudentSignUpId",
                table: "Questions",
                column: "StudentSignUpId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Questions_TutorAllocationId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "TutorAllocationId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_StudentSignUpId",
                table: "Questions");

            migrationBuilder.AlterColumn<int>(
                name: "StudentSignUpId",
                table: "Questions",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
