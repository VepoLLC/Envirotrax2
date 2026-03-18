using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sites",
                columns: table => new
                {
                    WaterSupplierId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubArea = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BusinessName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PropertyType = table.Column<int>(type: "int", nullable: true),
                    StreetNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StreetName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PropertyNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StateId = table.Column<int>(type: "int", nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MailingCompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MailingContactName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MailingStreetNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MailingStreetName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MailingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MailingCity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MailingStateId = table.Column<int>(type: "int", nullable: true),
                    MailingZipCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MailingPhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MailingEmailAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FogGeneratorPhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FogGeneratorEmailAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NeedsCsiInspection = table.Column<bool>(type: "bit", nullable: false),
                    CsiRenewalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NeedsBackflowLetter = table.Column<bool>(type: "bit", nullable: false),
                    BackflowLetterDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NeedsFogInspection = table.Column<bool>(type: "bit", nullable: false),
                    FogInspectionExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NeedsFogPermit = table.Column<bool>(type: "bit", nullable: false),
                    FogPermitExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastTripTicketDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TripTicketInterval = table.Column<int>(type: "int", nullable: false),
                    IsFeeExempt = table.Column<bool>(type: "bit", nullable: false),
                    RainFreezeSensorType = table.Column<int>(type: "int", nullable: false),
                    HasKnownBackflowAssemblies = table.Column<bool>(type: "bit", nullable: false),
                    HasOnSiteSewageFacility = table.Column<bool>(type: "bit", nullable: false),
                    HasWaterWell = table.Column<bool>(type: "bit", nullable: false),
                    HasAuxWaterSupply = table.Column<bool>(type: "bit", nullable: false),
                    HasFireSystem = table.Column<bool>(type: "bit", nullable: false),
                    FireSeparateWater = table.Column<bool>(type: "bit", nullable: false),
                    HasGreaseTrap = table.Column<int>(type: "int", nullable: false),
                    HasGritTrap = table.Column<bool>(type: "bit", nullable: false),
                    HasReclaimed = table.Column<bool>(type: "bit", nullable: false),
                    HasIrrigation = table.Column<bool>(type: "bit", nullable: false),
                    IrrigationSeparateWater = table.Column<bool>(type: "bit", nullable: false),
                    HasDomesticPremisesIsolation = table.Column<bool>(type: "bit", nullable: false),
                    RequiresDomesticPremisesIsolation = table.Column<bool>(type: "bit", nullable: false),
                    InvalidMailingAddress = table.Column<bool>(type: "bit", nullable: false),
                    OutOfArea = table.Column<bool>(type: "bit", nullable: false),
                    FacilityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FacilityMap = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    BackflowScheduleMonth = table.Column<int>(type: "int", nullable: false),
                    GisLatitude = table.Column<double>(type: "float", nullable: true),
                    GisLongitude = table.Column<double>(type: "float", nullable: true),
                    GisStatus = table.Column<int>(type: "int", nullable: false),
                    GisDate = table.Column<DateTime>(type: "date", nullable: true),
                    GisAreaId = table.Column<int>(type: "int", nullable: false),
                    GisOutOfArea = table.Column<bool>(type: "bit", nullable: false),
                    GisOutOfAreaCheckDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImportSiteId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ImportSiteId2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ImportId = table.Column<int>(type: "int", nullable: false),
                    ExcludeFromBackflowMailing = table.Column<bool>(type: "bit", nullable: false),
                    ExcludeFromCsiMailing = table.Column<bool>(type: "bit", nullable: false),
                    NeedsValidation = table.Column<bool>(type: "bit", nullable: false),
                    ValidationOnHold = table.Column<bool>(type: "bit", nullable: false),
                    BypassPropertyNumberValidation = table.Column<bool>(type: "bit", nullable: false),
                    UnknownAssemblyLettersSent = table.Column<int>(type: "int", nullable: false),
                    UnknownAssembliesLetterCount = table.Column<int>(type: "int", nullable: false),
                    UnknownAssembliesLetterStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomData1 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CustomBooleanData1 = table.Column<bool>(type: "bit", nullable: false),
                    UserAccountAssignment = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CsiAccountAssignment = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BackflowAccountAssignment = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FogAccountAssignment = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NeedsRenewalCheck = table.Column<bool>(type: "bit", nullable: false),
                    CsiAccountAssignmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BackflowAccountAssignmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FogAccountAssignmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => new { x.WaterSupplierId, x.Id });
                    table.ForeignKey(
                        name: "FK_Sites_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sites_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sites_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sites_States_MailingStateId",
                        column: x => x.MailingStateId,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sites_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sites_WaterSuppliers_WaterSupplierId",
                        column: x => x.WaterSupplierId,
                        principalTable: "WaterSuppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sites_CreatedById",
                table: "Sites",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_DeletedById",
                table: "Sites",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_MailingStateId",
                table: "Sites",
                column: "MailingStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_StateId",
                table: "Sites",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_UpdatedById",
                table: "Sites",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sites");
        }
    }
}
