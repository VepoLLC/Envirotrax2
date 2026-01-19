using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGeneralSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeneralSettings",
                columns: table => new
                {
                    WaterSupplierId = table.Column<int>(type: "int", nullable: false),
                    PrivacyRequired = table.Column<bool>(type: "bit", nullable: false),
                    NewSitesLocked = table.Column<bool>(type: "bit", nullable: false),
                    AdministrativeOnly = table.Column<bool>(type: "bit", nullable: false),
                    WiseGuys = table.Column<bool>(type: "bit", nullable: false),
                    BackflowTesting = table.Column<bool>(type: "bit", nullable: false),
                    CsiInspections = table.Column<bool>(type: "bit", nullable: false),
                    FogProgram = table.Column<bool>(type: "bit", nullable: false),
                    BpatsRequireInsurance = table.Column<bool>(type: "bit", nullable: false),
                    BpatsRequireIrrigationLicense = table.Column<bool>(type: "bit", nullable: false),
                    CsiInspectorsRequireInsurance = table.Column<bool>(type: "bit", nullable: false),
                    FogTransportersRequireInsurance = table.Column<bool>(type: "bit", nullable: false),
                    FogVehiclesRequirePermit = table.Column<bool>(type: "bit", nullable: false),
                    FogVehiclesRequireInspection = table.Column<bool>(type: "bit", nullable: false),
                    LockBpatRegistrations = table.Column<bool>(type: "bit", nullable: false),
                    LockCsiRegistrations = table.Column<bool>(type: "bit", nullable: false),
                    LockFogInspectorRegistrations = table.Column<bool>(type: "bit", nullable: false),
                    LockFogTransporterRegistrations = table.Column<bool>(type: "bit", nullable: false),
                    BpatsRequireInsuranceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CsiInspectorsRequireInsuranceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FogTransportersRequireInsuranceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BackflowCommercialTestFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BackflowCommercialTestFeeWsShare = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BackflowResidentialTestFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BackflowResidentialTestFeeWsShare = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CsiCommercialInspectionFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CsiCommercialInspectionFeeWsShare = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CsiResidentialInspectionFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CsiResidentialInspectionFeeWsShare = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FogTransportFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FogTransportFeeWsShare = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralSettings", x => x.WaterSupplierId);
                    table.ForeignKey(
                        name: "FK_GeneralSettings_WaterSuppliers_WaterSupplierId",
                        column: x => x.WaterSupplierId,
                        principalTable: "WaterSuppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneralSettings");
        }
    }
}
