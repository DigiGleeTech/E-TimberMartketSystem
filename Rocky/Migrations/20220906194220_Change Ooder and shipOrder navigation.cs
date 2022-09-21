using Microsoft.EntityFrameworkCore.Migrations;

namespace Rocky.Migrations
{
    public partial class ChangeOoderandshipOrdernavigation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_shipOrders_ShipOrderId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_ShipOrderId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ShipOrderId",
                table: "Order");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "shipOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_shipOrders_OrderId",
                table: "shipOrders",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_shipOrders_Order_OrderId",
                table: "shipOrders",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_shipOrders_Order_OrderId",
                table: "shipOrders");

            migrationBuilder.DropIndex(
                name: "IX_shipOrders_OrderId",
                table: "shipOrders");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "shipOrders");

            migrationBuilder.AddColumn<int>(
                name: "ShipOrderId",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Order_ShipOrderId",
                table: "Order",
                column: "ShipOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_shipOrders_ShipOrderId",
                table: "Order",
                column: "ShipOrderId",
                principalTable: "shipOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
