using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightBoostApplication.Data.Migrations
{
    public partial class termCourseSignUpUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourseSignUp_Student_StudentId",
                table: "StudentCourseSignUp");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourseSignUp_TermCourses_termCourseId",
                table: "StudentCourseSignUp");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentCourseSignUp",
                table: "StudentCourseSignUp");

            migrationBuilder.RenameTable(
                name: "StudentCourseSignUp",
                newName: "StudentCourseSignUps");

            migrationBuilder.RenameIndex(
                name: "IX_StudentCourseSignUp_termCourseId",
                table: "StudentCourseSignUps",
                newName: "IX_StudentCourseSignUps_termCourseId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentCourseSignUp_StudentId",
                table: "StudentCourseSignUps",
                newName: "IX_StudentCourseSignUps_StudentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentCourseSignUps",
                table: "StudentCourseSignUps",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourseSignUps_Student_StudentId",
                table: "StudentCourseSignUps",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourseSignUps_TermCourses_termCourseId",
                table: "StudentCourseSignUps",
                column: "termCourseId",
                principalTable: "TermCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourseSignUps_Student_StudentId",
                table: "StudentCourseSignUps");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourseSignUps_TermCourses_termCourseId",
                table: "StudentCourseSignUps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentCourseSignUps",
                table: "StudentCourseSignUps");

            migrationBuilder.RenameTable(
                name: "StudentCourseSignUps",
                newName: "StudentCourseSignUp");

            migrationBuilder.RenameIndex(
                name: "IX_StudentCourseSignUps_termCourseId",
                table: "StudentCourseSignUp",
                newName: "IX_StudentCourseSignUp_termCourseId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentCourseSignUps_StudentId",
                table: "StudentCourseSignUp",
                newName: "IX_StudentCourseSignUp_StudentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentCourseSignUp",
                table: "StudentCourseSignUp",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourseSignUp_Student_StudentId",
                table: "StudentCourseSignUp",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourseSignUp_TermCourses_termCourseId",
                table: "StudentCourseSignUp",
                column: "termCourseId",
                principalTable: "TermCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
