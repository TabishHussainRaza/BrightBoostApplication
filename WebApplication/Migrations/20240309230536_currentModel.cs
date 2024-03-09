using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication.Migrations
{
    public partial class currentModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_TransactionId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_TransactionId1",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_TransactionId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_TransactionId1",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TransactionId1",
                table: "Orders");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AddressId",
                table: "Orders",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TransactionId",
                table: "Orders",
                column: "TransactionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_AddressId",
                table: "Orders",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Transactions_TransactionId",
                table: "Orders",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_AddressId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Transactions_TransactionId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_AddressId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_TransactionId",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "TransactionId1",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TransactionId",
                table: "Orders",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TransactionId1",
                table: "Orders",
                column: "TransactionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_TransactionId",
                table: "Orders",
                column: "TransactionId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_TransactionId1",
                table: "Orders",
                column: "TransactionId1",
                principalTable: "Addresses",
                principalColumn: "Id");
        }
    }
}
