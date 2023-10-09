using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightBoostApplication.Data.Migrations
{
    public partial class RemoveForeignKeyToQuestions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_StudentSignUps_StudentSignUpId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_StudentSignUpId",
                table: "Questions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Questions_StudentSignUpId",
                table: "Questions",
                column: "StudentSignUpId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_StudentSignUps_StudentSignUpId",
                table: "Questions",
                column: "StudentSignUpId",
                principalTable: "StudentSignUps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
