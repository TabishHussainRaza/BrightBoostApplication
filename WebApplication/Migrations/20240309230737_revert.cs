using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication.Migrations
{
    public partial class revert : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Customers_CustomerId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Retailers_RetailerId",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_CategorySubTypes_CategoryTypes_ProductTypeId",
                table: "CategorySubTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_Products_ProductId",
                table: "Inventory");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Inventory_InventoryId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_AddressId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Transactions_TransactionId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_CategorySubTypes_CategorySubTypeId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Customers_CustomerId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Orders_OrderId1",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Retailers",
                table: "Retailers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customers",
                table: "Customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryTypes",
                table: "CategoryTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategorySubTypes",
                table: "CategorySubTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses");

            migrationBuilder.RenameTable(
                name: "Transactions",
                newName: "Transaction");

            migrationBuilder.RenameTable(
                name: "Retailers",
                newName: "Retailer");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Product");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "Order");

            migrationBuilder.RenameTable(
                name: "OrderItems",
                newName: "OrderItem");

            migrationBuilder.RenameTable(
                name: "Customers",
                newName: "Customer");

            migrationBuilder.RenameTable(
                name: "CategoryTypes",
                newName: "CategoryType");

            migrationBuilder.RenameTable(
                name: "CategorySubTypes",
                newName: "CategorySubType");

            migrationBuilder.RenameTable(
                name: "Addresses",
                newName: "Address");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_OrderId1",
                table: "Transaction",
                newName: "IX_Transaction_OrderId1");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_CustomerId",
                table: "Transaction",
                newName: "IX_Transaction_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CategorySubTypeId",
                table: "Product",
                newName: "IX_Product_CategorySubTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_TransactionId",
                table: "Order",
                newName: "IX_Order_TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CustomerId",
                table: "Order",
                newName: "IX_Order_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_AddressId",
                table: "Order",
                newName: "IX_Order_AddressId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItem",
                newName: "IX_OrderItem_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_InventoryId",
                table: "OrderItem",
                newName: "IX_OrderItem_InventoryId");

            migrationBuilder.RenameIndex(
                name: "IX_CategorySubTypes_ProductTypeId",
                table: "CategorySubType",
                newName: "IX_CategorySubType_ProductTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_CustomerId",
                table: "Address",
                newName: "IX_Address_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transaction",
                table: "Transaction",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Retailer",
                table: "Retailer",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Product",
                table: "Product",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Order",
                table: "Order",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItem",
                table: "OrderItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customer",
                table: "Customer",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryType",
                table: "CategoryType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategorySubType",
                table: "CategorySubType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Address",
                table: "Address",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Customer_CustomerId",
                table: "Address",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Retailer_RetailerId",
                table: "Branches",
                column: "RetailerId",
                principalTable: "Retailer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CategorySubType_CategoryType_ProductTypeId",
                table: "CategorySubType",
                column: "ProductTypeId",
                principalTable: "CategoryType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_Product_ProductId",
                table: "Inventory",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Address_AddressId",
                table: "Order",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Customer_CustomerId",
                table: "Order",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Transaction_TransactionId",
                table: "Order",
                column: "TransactionId",
                principalTable: "Transaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Inventory_InventoryId",
                table: "OrderItem",
                column: "InventoryId",
                principalTable: "Inventory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Order_OrderId",
                table: "OrderItem",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_CategorySubType_CategorySubTypeId",
                table: "Product",
                column: "CategorySubTypeId",
                principalTable: "CategorySubType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Customer_CustomerId",
                table: "Transaction",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Order_OrderId1",
                table: "Transaction",
                column: "OrderId1",
                principalTable: "Order",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Customer_CustomerId",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Retailer_RetailerId",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_CategorySubType_CategoryType_ProductTypeId",
                table: "CategorySubType");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_Product_ProductId",
                table: "Inventory");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Address_AddressId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Customer_CustomerId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Transaction_TransactionId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Inventory_InventoryId",
                table: "OrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Order_OrderId",
                table: "OrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_CategorySubType_CategorySubTypeId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Customer_CustomerId",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Order_OrderId1",
                table: "Transaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transaction",
                table: "Transaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Retailer",
                table: "Retailer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Product",
                table: "Product");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItem",
                table: "OrderItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Order",
                table: "Order");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customer",
                table: "Customer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryType",
                table: "CategoryType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategorySubType",
                table: "CategorySubType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Address",
                table: "Address");

            migrationBuilder.RenameTable(
                name: "Transaction",
                newName: "Transactions");

            migrationBuilder.RenameTable(
                name: "Retailer",
                newName: "Retailers");

            migrationBuilder.RenameTable(
                name: "Product",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "OrderItem",
                newName: "OrderItems");

            migrationBuilder.RenameTable(
                name: "Order",
                newName: "Orders");

            migrationBuilder.RenameTable(
                name: "Customer",
                newName: "Customers");

            migrationBuilder.RenameTable(
                name: "CategoryType",
                newName: "CategoryTypes");

            migrationBuilder.RenameTable(
                name: "CategorySubType",
                newName: "CategorySubTypes");

            migrationBuilder.RenameTable(
                name: "Address",
                newName: "Addresses");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_OrderId1",
                table: "Transactions",
                newName: "IX_Transactions_OrderId1");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_CustomerId",
                table: "Transactions",
                newName: "IX_Transactions_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_CategorySubTypeId",
                table: "Products",
                newName: "IX_Products_CategorySubTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_OrderId",
                table: "OrderItems",
                newName: "IX_OrderItems_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_InventoryId",
                table: "OrderItems",
                newName: "IX_OrderItems_InventoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_TransactionId",
                table: "Orders",
                newName: "IX_Orders_TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_CustomerId",
                table: "Orders",
                newName: "IX_Orders_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_AddressId",
                table: "Orders",
                newName: "IX_Orders_AddressId");

            migrationBuilder.RenameIndex(
                name: "IX_CategorySubType_ProductTypeId",
                table: "CategorySubTypes",
                newName: "IX_CategorySubTypes_ProductTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Address_CustomerId",
                table: "Addresses",
                newName: "IX_Addresses_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Retailers",
                table: "Retailers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customers",
                table: "Customers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryTypes",
                table: "CategoryTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategorySubTypes",
                table: "CategorySubTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Customers_CustomerId",
                table: "Addresses",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Retailers_RetailerId",
                table: "Branches",
                column: "RetailerId",
                principalTable: "Retailers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CategorySubTypes_CategoryTypes_ProductTypeId",
                table: "CategorySubTypes",
                column: "ProductTypeId",
                principalTable: "CategoryTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_Products_ProductId",
                table: "Inventory",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Inventory_InventoryId",
                table: "OrderItems",
                column: "InventoryId",
                principalTable: "Inventory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_AddressId",
                table: "Orders",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Transactions_TransactionId",
                table: "Orders",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CategorySubTypes_CategorySubTypeId",
                table: "Products",
                column: "CategorySubTypeId",
                principalTable: "CategorySubTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Customers_CustomerId",
                table: "Transactions",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Orders_OrderId1",
                table: "Transactions",
                column: "OrderId1",
                principalTable: "Orders",
                principalColumn: "Id");
        }
    }
}
