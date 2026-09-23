using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWaterSupplierUserRolesNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_WaterSupplierUsers_WaterSupplierId_UserId",
                table: "UserRoles",
                columns: new[] { "WaterSupplierId", "UserId" },
                principalTable: "WaterSupplierUsers",
                principalColumns: new[] { "WaterSupplierId", "UserId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_WaterSupplierUsers_WaterSupplierId_UserId",
                table: "UserRoles");
        }
    }
}
