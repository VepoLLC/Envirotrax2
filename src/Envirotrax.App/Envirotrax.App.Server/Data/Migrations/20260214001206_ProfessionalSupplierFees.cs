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
                newName: "CsiResidentialInspectionFee");

            migrationBuilder.RenameColumn(
                name: "CommercialFee",
                table: "ProfessionalWaterSuppliers",
                newName: "CsiCommercialInspectionFee");

            migrationBuilder.AddColumn<decimal>(
                name: "BackflowCommercialTestFee",
                table: "ProfessionalWaterSuppliers",
                type: "decimal(19,4)",
                precision: 19,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BackflowResidentialTestFee",
                table: "ProfessionalWaterSuppliers",
                type: "decimal(19,4)",
                precision: 19,
                scale: 4,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackflowCommercialTestFee",
                table: "ProfessionalWaterSuppliers");

            migrationBuilder.DropColumn(
                name: "BackflowResidentialTestFee",
                table: "ProfessionalWaterSuppliers");

            migrationBuilder.RenameColumn(
                name: "FogTransportFee",
                table: "ProfessionalWaterSuppliers",
                newName: "ResidentialFee");

            migrationBuilder.RenameColumn(
                name: "CsiResidentialInspectionFee",
                table: "ProfessionalWaterSuppliers",
                newName: "FogFee");

            migrationBuilder.RenameColumn(
                name: "CsiCommercialInspectionFee",
                table: "ProfessionalWaterSuppliers",
                newName: "CommercialFee");
        }
    }
}
