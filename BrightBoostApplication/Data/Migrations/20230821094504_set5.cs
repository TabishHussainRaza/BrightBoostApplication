using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrightBoostApplication.Data.Migrations
{
    public partial class set5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Session_TermCourses_TermCourseId",
                table: "Session");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSignUp_Session_SessionId",
                table: "StudentSignUp");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSignUp_Student_StudentId",
                table: "StudentSignUp");

            migrationBuilder.DropForeignKey(
                name: "FK_TutorAllocation_Session_SessionId",
                table: "TutorAllocation");

            migrationBuilder.DropForeignKey(
                name: "FK_TutorAllocation_Tutors_TutorId",
                table: "TutorAllocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TutorAllocation",
                table: "TutorAllocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentSignUp",
                table: "StudentSignUp");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Session",
                table: "Session");

            migrationBuilder.RenameTable(
                name: "TutorAllocation",
                newName: "TutorAllocations");

            migrationBuilder.RenameTable(
                name: "StudentSignUp",
                newName: "StudentSignUps");

            migrationBuilder.RenameTable(
                name: "Session",
                newName: "Sessions");

            migrationBuilder.RenameIndex(
                name: "IX_TutorAllocation_TutorId",
                table: "TutorAllocations",
                newName: "IX_TutorAllocations_TutorId");

            migrationBuilder.RenameIndex(
                name: "IX_TutorAllocation_SessionId",
                table: "TutorAllocations",
                newName: "IX_TutorAllocations_SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSignUp_StudentId",
                table: "StudentSignUps",
                newName: "IX_StudentSignUps_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSignUp_SessionId",
                table: "StudentSignUps",
                newName: "IX_StudentSignUps_SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_Session_TermCourseId",
                table: "Sessions",
                newName: "IX_Sessions_TermCourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TutorAllocations",
                table: "TutorAllocations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentSignUps",
                table: "StudentSignUps",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttendanceDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<bool>(type: "bit", nullable: true),
                    order = table.Column<int>(type: "int", nullable: false),
                    fkId = table.Column<int>(type: "int", nullable: false),
                    StudentSignUpId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.id);
                    table.ForeignKey(
                        name: "FK_Attendances_StudentSignUps_StudentSignUpId",
                        column: x => x.StudentSignUpId,
                        principalTable: "StudentSignUps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<bool>(type: "bit", nullable: true),
                    order = table.Column<int>(type: "int", nullable: false),
                    fkId = table.Column<int>(type: "int", nullable: false),
                    StudentSignUpId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.id);
                    table.ForeignKey(
                        name: "FK_Questions_StudentSignUps_StudentSignUpId",
                        column: x => x.StudentSignUpId,
                        principalTable: "StudentSignUps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_StudentSignUpId",
                table: "Attendances",
                column: "StudentSignUpId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_StudentSignUpId",
                table: "Questions",
                column: "StudentSignUpId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_TermCourses_TermCourseId",
                table: "Sessions",
                column: "TermCourseId",
                principalTable: "TermCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSignUps_Sessions_SessionId",
                table: "StudentSignUps",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSignUps_Student_StudentId",
                table: "StudentSignUps",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TutorAllocations_Sessions_SessionId",
                table: "TutorAllocations",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TutorAllocations_Tutors_TutorId",
                table: "TutorAllocations",
                column: "TutorId",
                principalTable: "Tutors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_TermCourses_TermCourseId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSignUps_Sessions_SessionId",
                table: "StudentSignUps");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSignUps_Student_StudentId",
                table: "StudentSignUps");

            migrationBuilder.DropForeignKey(
                name: "FK_TutorAllocations_Sessions_SessionId",
                table: "TutorAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_TutorAllocations_Tutors_TutorId",
                table: "TutorAllocations");

            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TutorAllocations",
                table: "TutorAllocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentSignUps",
                table: "StudentSignUps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions");

            migrationBuilder.RenameTable(
                name: "TutorAllocations",
                newName: "TutorAllocation");

            migrationBuilder.RenameTable(
                name: "StudentSignUps",
                newName: "StudentSignUp");

            migrationBuilder.RenameTable(
                name: "Sessions",
                newName: "Session");

            migrationBuilder.RenameIndex(
                name: "IX_TutorAllocations_TutorId",
                table: "TutorAllocation",
                newName: "IX_TutorAllocation_TutorId");

            migrationBuilder.RenameIndex(
                name: "IX_TutorAllocations_SessionId",
                table: "TutorAllocation",
                newName: "IX_TutorAllocation_SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSignUps_StudentId",
                table: "StudentSignUp",
                newName: "IX_StudentSignUp_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSignUps_SessionId",
                table: "StudentSignUp",
                newName: "IX_StudentSignUp_SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_TermCourseId",
                table: "Session",
                newName: "IX_Session_TermCourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TutorAllocation",
                table: "TutorAllocation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentSignUp",
                table: "StudentSignUp",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Session",
                table: "Session",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Session_TermCourses_TermCourseId",
                table: "Session",
                column: "TermCourseId",
                principalTable: "TermCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSignUp_Session_SessionId",
                table: "StudentSignUp",
                column: "SessionId",
                principalTable: "Session",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSignUp_Student_StudentId",
                table: "StudentSignUp",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TutorAllocation_Session_SessionId",
                table: "TutorAllocation",
                column: "SessionId",
                principalTable: "Session",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TutorAllocation_Tutors_TutorId",
                table: "TutorAllocation",
                column: "TutorId",
                principalTable: "Tutors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
