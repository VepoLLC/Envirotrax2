using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLegacyRecordIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LegacyRecordId",
                table: "WaterSuppliers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LegacyRecordId",
                table: "Sites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LegacyRecordId",
                table: "BackflowTests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WaterSuppliers_LegacyRecordId",
                table: "WaterSuppliers",
                column: "LegacyRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_LegacyRecordId",
                table: "Sites",
                column: "LegacyRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_BackflowTests_LegacyRecordId",
                table: "BackflowTests",
                column: "LegacyRecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WaterSuppliers_LegacyRecordId",
                table: "WaterSuppliers");

            migrationBuilder.DropIndex(
                name: "IX_Sites_LegacyRecordId",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_BackflowTests_LegacyRecordId",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "LegacyRecordId",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "LegacyRecordId",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "LegacyRecordId",
                table: "BackflowTests");
        }
    }
}
