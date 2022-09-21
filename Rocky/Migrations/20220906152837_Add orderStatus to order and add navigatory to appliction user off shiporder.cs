using Microsoft.EntityFrameworkCore.Migrations;

namespace Rocky.Migrations
{
    public partial class AddorderStatustoorderandaddnavigatorytoapplictionuseroffshiporder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "OrderStatus",
                table: "Order",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ShipOrderId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ShipOrderId",
                table: "AspNetUsers",
                column: "ShipOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_shipOrders_ShipOrderId",
                table: "AspNetUsers",
                column: "ShipOrderId",
                principalTable: "shipOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_shipOrders_ShipOrderId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ShipOrderId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ShipOrderId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "OrderStatus",
                table: "Order",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
