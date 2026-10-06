using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDomainIndexAndRemoveLegacyRecordId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WaterSuppliers_LegacyRecordId",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "LegacyRecordId",
                table: "WaterSuppliers");

            migrationBuilder.CreateIndex(
                name: "IX_WaterSuppliers_Domain",
                table: "WaterSuppliers",
                column: "Domain");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WaterSuppliers_Domain",
                table: "WaterSuppliers");

            migrationBuilder.AddColumn<int>(
                name: "LegacyRecordId",
                table: "WaterSuppliers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WaterSuppliers_LegacyRecordId",
                table: "WaterSuppliers",
                column: "LegacyRecordId");
        }
    }
}
