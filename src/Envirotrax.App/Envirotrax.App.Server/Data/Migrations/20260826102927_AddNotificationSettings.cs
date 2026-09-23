using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationSettings",
                columns: table => new
                {
                    WaterSupplierId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    ReasonForTest = table.Column<int>(type: "int", nullable: true),
                    PropertyTypeResidential = table.Column<bool>(type: "bit", nullable: false),
                    PropertyTypeCommercial = table.Column<bool>(type: "bit", nullable: false),
                    PropertyTypeAny = table.Column<bool>(type: "bit", nullable: false),
                    FilterFailedTest = table.Column<bool>(type: "bit", nullable: false),
                    FilterPassingTest = table.Column<bool>(type: "bit", nullable: false),
                    FilterUnknownSerialNumber = table.Column<bool>(type: "bit", nullable: false),
                    FilterInactiveProperty = table.Column<bool>(type: "bit", nullable: false),
                    FilterNonCompliance = table.Column<bool>(type: "bit", nullable: false),
                    FilterPotableNonPotableMismatch = table.Column<bool>(type: "bit", nullable: false),
                    FilterDuplicateTest = table.Column<bool>(type: "bit", nullable: false),
                    FilterOutOfService = table.Column<bool>(type: "bit", nullable: false),
                    FilterContainsRemarks = table.Column<bool>(type: "bit", nullable: false),
                    FilterBackflowNotProperlyInstalled = table.Column<bool>(type: "bit", nullable: false),
                    FilterFeeExempt = table.Column<bool>(type: "bit", nullable: false),
                    FilterHasOnSiteSewageFacility = table.Column<bool>(type: "bit", nullable: false),
                    FilterHasAuxWaterSupply = table.Column<bool>(type: "bit", nullable: false),
                    FilterSubmissionDaysExceeded = table.Column<bool>(type: "bit", nullable: false),
                    FilterSubmissionDaysExceededDays = table.Column<int>(type: "int", nullable: false),
                    FilterAny = table.Column<bool>(type: "bit", nullable: false),
                    HazardTypeAgriculturalFeedLot = table.Column<bool>(type: "bit", nullable: false),
                    HazardTypeDomesticPremisesIsolation = table.Column<bool>(type: "bit", nullable: false),
                    HazardTypeFireSystem = table.Column<bool>(type: "bit", nullable: false),
                    HazardTypeFireHydrantTemporaryConstruction = table.Column<bool>(type: "bit", nullable: false),
                    HazardTypeGasStationCarWash = table.Column<bool>(type: "bit", nullable: false),
                    HazardTypeIrrigationNonChemical = table.Column<bool>(type: "bit", nullable: false),
                    HazardTypeIrrigationChemicalFeed = table.Column<bool>(type: "bit", nullable: false),
                    HazardTypeLaundryCleaners = table.Column<bool>(type: "bit", nullable: false),
                    HazardTypeMedicalDentalLaboratoryMortuary = table.Column<bool>(type: "bit", nullable: false),
                    HazardTypeNailsSalonGrooming = table.Column<bool>(type: "bit", nullable: false),
                    HazardTypePoolRecreationAthletics = table.Column<bool>(type: "bit", nullable: false),
                    HazardTypeRestaurantVendingGrocery = table.Column<bool>(type: "bit", nullable: false),
                    HazardTypeFountainsGardenPondsWaterFeatures = table.Column<bool>(type: "bit", nullable: false),
                    HazardTypeWaterSoftener = table.Column<bool>(type: "bit", nullable: false),
                    HazardTypeOther = table.Column<bool>(type: "bit", nullable: false),
                    HazardTypeAny = table.Column<bool>(type: "bit", nullable: false),
                    Interval = table.Column<int>(type: "int", nullable: false),
                    DeliveryType = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSettings", x => new { x.WaterSupplierId, x.Id });
                    table.ForeignKey(
                        name: "FK_NotificationSettings_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationSettings_WaterSupplierUsers_WaterSupplierId_UserId",
                        columns: x => new { x.WaterSupplierId, x.UserId },
                        principalTable: "WaterSupplierUsers",
                        principalColumns: new[] { "WaterSupplierId", "UserId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationSettings_WaterSuppliers_WaterSupplierId",
                        column: x => x.WaterSupplierId,
                        principalTable: "WaterSuppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSettings_CreatedById",
                table: "NotificationSettings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSettings_WaterSupplierId_UserId",
                table: "NotificationSettings",
                columns: new[] { "WaterSupplierId", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationSettings");
        }
    }
}
