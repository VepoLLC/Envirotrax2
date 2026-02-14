using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProfessionalSupplierFees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResidentialFee",
                table: "ProfessionalWaterSuppliers",
                newName: "FogTransportFee");

            migrationBuilder.RenameColumn(
                name: "FogFee",
                table: "ProfessionalWaterSuppliers",
                newName: "BackflowResidentialTestFee");

            migrationBuilder.RenameColumn(
                name: "CommercialFee",
                table: "ProfessionalWaterSuppliers",
                newName: "BackflowCommercialTestFee");

            migrationBuilder.AddColumn<decimal>(
                name: "CsiCommercialInspectionFee",
                table: "ProfessionalWaterSuppliers",
                type: "decimal(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CsiResidentialInspectionFee",
                table: "ProfessionalWaterSuppliers",
                type: "decimal(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CsiCommercialInspectionFee",
                table: "ProfessionalWaterSuppliers");

            migrationBuilder.DropColumn(
                name: "CsiResidentialInspectionFee",
                table: "ProfessionalWaterSuppliers");

            migrationBuilder.RenameColumn(
                name: "FogTransportFee",
                table: "ProfessionalWaterSuppliers",
                newName: "ResidentialFee");

            migrationBuilder.RenameColumn(
                name: "BackflowResidentialTestFee",
                table: "ProfessionalWaterSuppliers",
                newName: "FogFee");

            migrationBuilder.RenameColumn(
                name: "BackflowCommercialTestFee",
                table: "ProfessionalWaterSuppliers",
                newName: "CommercialFee");
        }
    }
}
