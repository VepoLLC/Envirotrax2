using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class removeParentIdFromNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_WaterSuppliers_ParentWaterSupplierId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ParentWaterSupplierId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ParentWaterSupplierId",
                table: "Notifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentWaterSupplierId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ParentWaterSupplierId",
                table: "Notifications",
                column: "ParentWaterSupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_WaterSuppliers_ParentWaterSupplierId",
                table: "Notifications",
                column: "ParentWaterSupplierId",
                principalTable: "WaterSuppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
