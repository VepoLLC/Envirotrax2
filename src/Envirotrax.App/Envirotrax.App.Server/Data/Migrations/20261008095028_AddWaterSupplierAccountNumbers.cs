using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWaterSupplierAccountNumbers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WaterSupplierAccountNumber",
                table: "Sites",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IncludeWsAccountNumbers",
                table: "GeneralSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseWsAccountNumbersOnLetters",
                table: "GeneralSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Sites_WaterSupplierId_WaterSupplierAccountNumber",
                table: "Sites",
                columns: new[] { "WaterSupplierId", "WaterSupplierAccountNumber" },
                unique: true,
                filter: "[WaterSupplierAccountNumber] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sites_WaterSupplierId_WaterSupplierAccountNumber",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "WaterSupplierAccountNumber",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "IncludeWsAccountNumbers",
                table: "GeneralSettings");

            migrationBuilder.DropColumn(
                name: "UseWsAccountNumbersOnLetters",
                table: "GeneralSettings");
        }
    }
}
