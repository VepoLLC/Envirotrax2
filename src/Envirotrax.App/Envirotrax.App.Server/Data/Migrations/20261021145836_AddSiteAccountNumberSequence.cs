using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteAccountNumberSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sites_WaterSupplierId_WaterSupplierAccountNumber",
                table: "Sites");

            migrationBuilder.CreateSequence(
                name: "SiteAccountNumberSequence",
                startValue: 100000L);

            migrationBuilder.AddColumn<int>(
                name: "AccountNumberDuplicateIndex",
                table: "Sites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WaterSupplierAccountNumberDuplicateIndex",
                table: "Sites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sites_WaterSupplierId_AccountNumber_AccountNumberDuplicateIndex",
                table: "Sites",
                columns: new[] { "WaterSupplierId", "AccountNumber", "AccountNumberDuplicateIndex" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sites_WaterSupplierId_WaterSupplierAccountNumber_WaterSupplierAccountNumberDuplicateIndex",
                table: "Sites",
                columns: new[] { "WaterSupplierId", "WaterSupplierAccountNumber", "WaterSupplierAccountNumberDuplicateIndex" },
                unique: true,
                filter: "[WaterSupplierAccountNumber] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sites_WaterSupplierId_AccountNumber_AccountNumberDuplicateIndex",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_Sites_WaterSupplierId_WaterSupplierAccountNumber_WaterSupplierAccountNumberDuplicateIndex",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "AccountNumberDuplicateIndex",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "WaterSupplierAccountNumberDuplicateIndex",
                table: "Sites");

            migrationBuilder.DropSequence(
                name: "SiteAccountNumberSequence");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_WaterSupplierId_WaterSupplierAccountNumber",
                table: "Sites",
                columns: new[] { "WaterSupplierId", "WaterSupplierAccountNumber" },
                unique: true,
                filter: "[WaterSupplierAccountNumber] IS NOT NULL");
        }
    }
}
