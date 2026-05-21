using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFogInspections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FogInspections",
                columns: table => new
                {
                    WaterSupplierId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteId = table.Column<int>(type: "int", nullable: false),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubmissionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PropertyBusinessName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PropertyType = table.Column<int>(type: "int", nullable: false),
                    PropertyStreetNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PropertyStreetName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PropertyNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PropertyCity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PropertyStateId = table.Column<int>(type: "int", nullable: true),
                    PropertyZip = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MailingCompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MailingContactName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MailingStreetNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MailingStreetName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MailingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MailingCity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MailingStateId = table.Column<int>(type: "int", nullable: true),
                    MailingZip = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MailingPhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MailingEmailAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MasterInspectorId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InspectorId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InspectorCompanyName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    InspectorJobTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InspectorContactName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    InspectorAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    InspectorCity = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    InspectorState = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    InspectorZip = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    InspectorWorkNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InspectorCellNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InspectorFaxNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FogGeneratorPhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FogGeneratorEmailAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FacilityType = table.Column<int>(type: "int", nullable: false),
                    ReasonForInspection = table.Column<int>(type: "int", nullable: false),
                    InterceptorType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InterceptorOtherDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InterceptorCapacity = table.Column<int>(type: "int", nullable: false),
                    InterceptorCapacityType = table.Column<int>(type: "int", nullable: false),
                    InterceptorLocationDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InterceptorLatitude = table.Column<double>(type: "float", nullable: true),
                    InterceptorLongitude = table.Column<double>(type: "float", nullable: true),
                    InterceptorComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Maintained = table.Column<bool>(type: "bit", nullable: false),
                    Accessible = table.Column<bool>(type: "bit", nullable: false),
                    PastOverflow = table.Column<bool>(type: "bit", nullable: false),
                    InletChamberWettingHeight = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InletChamberGreaseBlanket = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InletChamberSediments = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OutletChamberWettingHeight = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OutletChamberGreaseBlanket = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OutletChamberSediments = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InletTeeIntact = table.Column<bool>(type: "bit", nullable: false),
                    OutletTeeIntact = table.Column<bool>(type: "bit", nullable: false),
                    InletTeeVisible = table.Column<bool>(type: "bit", nullable: true),
                    OutletTeeVisible = table.Column<bool>(type: "bit", nullable: true),
                    SampledFrom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SamplingPointAccessible = table.Column<bool>(type: "bit", nullable: false),
                    SamplingPointClean = table.Column<bool>(type: "bit", nullable: false),
                    InletTotalCapacityPercent = table.Column<int>(type: "int", nullable: false),
                    OutletTotalCapacityPercent = table.Column<int>(type: "int", nullable: false),
                    TotalCapacityPercent = table.Column<int>(type: "int", nullable: false),
                    InspectionResult = table.Column<int>(type: "int", nullable: false),
                    SignatureContactName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SignatureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTripTicketDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TripTicketInterval = table.Column<int>(type: "int", nullable: false),
                    NeedsValidation = table.Column<bool>(type: "bit", nullable: false),
                    ValidationOnHold = table.Column<bool>(type: "bit", nullable: false),
                    ValidationNewSite = table.Column<bool>(type: "bit", nullable: false),
                    ValidationSiteInformationChanged = table.Column<bool>(type: "bit", nullable: false),
                    ValidatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ValidationClearedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidationLockedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidationNotes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    AmountShare = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    EmailPdf = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FogInspections", x => new { x.WaterSupplierId, x.Id });
                    table.ForeignKey(
                        name: "FK_FogInspections_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FogInspections_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FogInspections_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FogInspections_Sites_WaterSupplierId_SiteId",
                        columns: x => new { x.WaterSupplierId, x.SiteId },
                        principalTable: "Sites",
                        principalColumns: new[] { "WaterSupplierId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FogInspections_States_MailingStateId",
                        column: x => x.MailingStateId,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FogInspections_States_PropertyStateId",
                        column: x => x.PropertyStateId,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FogInspections_WaterSuppliers_WaterSupplierId",
                        column: x => x.WaterSupplierId,
                        principalTable: "WaterSuppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FogInspections_CreatedById",
                table: "FogInspections",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FogInspections_DeletedById",
                table: "FogInspections",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_FogInspections_MailingStateId",
                table: "FogInspections",
                column: "MailingStateId");

            migrationBuilder.CreateIndex(
                name: "IX_FogInspections_PropertyStateId",
                table: "FogInspections",
                column: "PropertyStateId");

            migrationBuilder.CreateIndex(
                name: "IX_FogInspections_UpdatedById",
                table: "FogInspections",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FogInspections_WaterSupplierId_SiteId",
                table: "FogInspections",
                columns: new[] { "WaterSupplierId", "SiteId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FogInspections");
        }
    }
}
