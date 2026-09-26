using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProfessionalWaterSupplierPerServiceSuspension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "ProfessionalWaterSuppliers");

            migrationBuilder.AddColumn<bool>(
                name: "IsBackflowTestingSuspended",
                table: "ProfessionalWaterSuppliers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCsiInspectionSuspended",
                table: "ProfessionalWaterSuppliers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFogInspectionSuspended",
                table: "ProfessionalWaterSuppliers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFogTransportationSuspended",
                table: "ProfessionalWaterSuppliers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBackflowTestingSuspended",
                table: "ProfessionalWaterSuppliers");

            migrationBuilder.DropColumn(
                name: "IsCsiInspectionSuspended",
                table: "ProfessionalWaterSuppliers");

            migrationBuilder.DropColumn(
                name: "IsFogInspectionSuspended",
                table: "ProfessionalWaterSuppliers");

            migrationBuilder.DropColumn(
                name: "IsFogTransportationSuspended",
                table: "ProfessionalWaterSuppliers");

            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "ProfessionalWaterSuppliers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
