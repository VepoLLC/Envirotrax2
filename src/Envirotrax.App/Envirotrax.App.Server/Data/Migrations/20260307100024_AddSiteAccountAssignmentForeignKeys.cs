using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteAccountAssignmentForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Sites_WaterSupplierId_BackflowAccountAssignmentId",
                table: "Sites",
                columns: new[] { "WaterSupplierId", "BackflowAccountAssignmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Sites_WaterSupplierId_CsiAccountAssignmentId",
                table: "Sites",
                columns: new[] { "WaterSupplierId", "CsiAccountAssignmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Sites_WaterSupplierId_FogAccountAssignmentId",
                table: "Sites",
                columns: new[] { "WaterSupplierId", "FogAccountAssignmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Sites_WaterSupplierId_UserAccountAssignmentId",
                table: "Sites",
                columns: new[] { "WaterSupplierId", "UserAccountAssignmentId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Sites_WaterSupplierUsers_WaterSupplierId_BackflowAccountAssignmentId",
                table: "Sites",
                columns: new[] { "WaterSupplierId", "BackflowAccountAssignmentId" },
                principalTable: "WaterSupplierUsers",
                principalColumns: new[] { "WaterSupplierId", "UserId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sites_WaterSupplierUsers_WaterSupplierId_CsiAccountAssignmentId",
                table: "Sites",
                columns: new[] { "WaterSupplierId", "CsiAccountAssignmentId" },
                principalTable: "WaterSupplierUsers",
                principalColumns: new[] { "WaterSupplierId", "UserId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sites_WaterSupplierUsers_WaterSupplierId_FogAccountAssignmentId",
                table: "Sites",
                columns: new[] { "WaterSupplierId", "FogAccountAssignmentId" },
                principalTable: "WaterSupplierUsers",
                principalColumns: new[] { "WaterSupplierId", "UserId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sites_WaterSupplierUsers_WaterSupplierId_UserAccountAssignmentId",
                table: "Sites",
                columns: new[] { "WaterSupplierId", "UserAccountAssignmentId" },
                principalTable: "WaterSupplierUsers",
                principalColumns: new[] { "WaterSupplierId", "UserId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sites_WaterSupplierUsers_WaterSupplierId_BackflowAccountAssignmentId",
                table: "Sites");

            migrationBuilder.DropForeignKey(
                name: "FK_Sites_WaterSupplierUsers_WaterSupplierId_CsiAccountAssignmentId",
                table: "Sites");

            migrationBuilder.DropForeignKey(
                name: "FK_Sites_WaterSupplierUsers_WaterSupplierId_FogAccountAssignmentId",
                table: "Sites");

            migrationBuilder.DropForeignKey(
                name: "FK_Sites_WaterSupplierUsers_WaterSupplierId_UserAccountAssignmentId",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_Sites_WaterSupplierId_BackflowAccountAssignmentId",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_Sites_WaterSupplierId_CsiAccountAssignmentId",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_Sites_WaterSupplierId_FogAccountAssignmentId",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_Sites_WaterSupplierId_UserAccountAssignmentId",
                table: "Sites");
        }
    }
}
