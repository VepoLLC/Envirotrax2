using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFogTripTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FogTripTickets",
                columns: table => new
                {
                    WaterSupplierId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmissionId = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    SiteId = table.Column<int>(type: "int", nullable: false),
                    PropertyBusinessName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PropertyType = table.Column<int>(type: "int", nullable: false),
                    PropertyStreetNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PropertyStreetName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PropertyNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PropertyCity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PropertyStateId = table.Column<int>(type: "int", nullable: true),
                    PropertyZip = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FogGeneratorPhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FogGeneratorEmailAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FogGeneratorContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProfessionalId = table.Column<int>(type: "int", nullable: false),
                    TransporterLicenseNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TransporterLicenseExpiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TransporterCompanyName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TransporterContactName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TransporterAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TransporterCity = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TransporterState = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TransporterZip = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TransporterWorkNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TransporterCellNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TransporterFaxNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TransporterEmailAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    GeneratorContactName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    GeneratorSignaturePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GeneratorSignatureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InterceptorType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InterceptorOtherDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InterceptorCapacity = table.Column<double>(type: "float", nullable: false),
                    InterceptorCapacityType = table.Column<int>(type: "int", nullable: false),
                    InterceptorWasteRemovedAmount = table.Column<double>(type: "float", nullable: false),
                    InterceptorWasteRemovedType = table.Column<int>(type: "int", nullable: false),
                    InterceptorWasteRemovedAmountGallons = table.Column<double>(type: "float", nullable: false),
                    InterceptorWasteRemovedAmountCubicFeet = table.Column<double>(type: "float", nullable: false),
                    InterceptorWasteRemovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VehicleId = table.Column<int>(type: "int", nullable: true),
                    VehicleLicensePlateNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    VehicleManufacturer = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    VehicleYear = table.Column<int>(type: "int", nullable: false),
                    VehicleCapacity = table.Column<double>(type: "float", nullable: false),
                    VehicleCapacityType = table.Column<int>(type: "int", nullable: false),
                    VehicleStickerNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VehiclePermitNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReceiverSiteId = table.Column<int>(type: "int", nullable: true),
                    ReceiverDisposalSiteId = table.Column<int>(type: "int", nullable: true),
                    ReceiverCompanyName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ReceiverContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReceiverAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ReceiverCity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReceiverState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReceiverZip = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReceiverPhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReceiverEmailAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ReceiverRegistrationNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReceiverPermitNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReceiverWasteDeliveredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceiverSignaturePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReceiverSignatureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PickupCompleted = table.Column<bool>(type: "bit", nullable: false),
                    Completed = table.Column<bool>(type: "bit", nullable: false),
                    Disapproved = table.Column<bool>(type: "bit", nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NeedsValidation = table.Column<bool>(type: "bit", nullable: false),
                    ValidationOnHold = table.Column<bool>(type: "bit", nullable: false),
                    ValidatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ValidationClearedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidationLockedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidationNewSite = table.Column<bool>(type: "bit", nullable: false),
                    ValidationSiteInformationChanged = table.Column<bool>(type: "bit", nullable: false),
                    ValidationReceiverInformationChanged = table.Column<bool>(type: "bit", nullable: false),
                    ValidationNotes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    AmountShare = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    WsPaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SalesRepPaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailPdf = table.Column<bool>(type: "bit", nullable: false),
                    EmailStatus = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FogTripTickets", x => new { x.WaterSupplierId, x.Id });
                    table.ForeignKey(
                        name: "FK_FogTripTickets_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FogTripTickets_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FogTripTickets_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FogTripTickets_FogDisposalSites_ReceiverDisposalSiteId",
                        column: x => x.ReceiverDisposalSiteId,
                        principalTable: "FogDisposalSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FogTripTickets_FogVehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "FogVehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FogTripTickets_Professionals_ProfessionalId",
                        column: x => x.ProfessionalId,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FogTripTickets_Sites_WaterSupplierId_SiteId",
                        columns: x => new { x.WaterSupplierId, x.SiteId },
                        principalTable: "Sites",
                        principalColumns: new[] { "WaterSupplierId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FogTripTickets_States_PropertyStateId",
                        column: x => x.PropertyStateId,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FogTripTickets_WaterSuppliers_WaterSupplierId",
                        column: x => x.WaterSupplierId,
                        principalTable: "WaterSuppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FogTripTickets_CreatedById",
                table: "FogTripTickets",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FogTripTickets_DeletedById",
                table: "FogTripTickets",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_FogTripTickets_ProfessionalId",
                table: "FogTripTickets",
                column: "ProfessionalId");

            migrationBuilder.CreateIndex(
                name: "IX_FogTripTickets_PropertyStateId",
                table: "FogTripTickets",
                column: "PropertyStateId");

            migrationBuilder.CreateIndex(
                name: "IX_FogTripTickets_ReceiverDisposalSiteId",
                table: "FogTripTickets",
                column: "ReceiverDisposalSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_FogTripTickets_UpdatedById",
                table: "FogTripTickets",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FogTripTickets_VehicleId",
                table: "FogTripTickets",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_FogTripTickets_WaterSupplierId_SiteId",
                table: "FogTripTickets",
                columns: new[] { "WaterSupplierId", "SiteId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FogTripTickets");
        }
    }
}
