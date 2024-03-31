using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication.Migrations
{
    public partial class updates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Retailers_RetailerId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "ReatilerId",
                table: "Branches");

            migrationBuilder.AlterColumn<int>(
                name: "RetailerId",
                table: "Branches",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Retailers_RetailerId",
                table: "Branches",
                column: "RetailerId",
                principalTable: "Retailers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Retailers_RetailerId",
                table: "Branches");

            migrationBuilder.AlterColumn<int>(
                name: "RetailerId",
                table: "Branches",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ReatilerId",
                table: "Branches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Retailers_RetailerId",
                table: "Branches",
                column: "RetailerId",
                principalTable: "Retailers",
                principalColumn: "Id");
        }
    }
}
