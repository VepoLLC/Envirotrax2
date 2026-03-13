using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCsiInspections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CsiInspections",
                columns: table => new
                {
                    WaterSupplierId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteId = table.Column<int>(type: "int", nullable: true),
                    InspectionDate = table.Column<DateTime>(type: "date", nullable: true),
                    SubmissionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PropertyBusinessName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PropertyType = table.Column<int>(type: "int", nullable: false),
                    PropertyStreetNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PropertyStreetName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PropertyNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PropertyCity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PropertyState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PropertyZip = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MailingCompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MailingContactName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MailingStreetNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MailingStreetName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MailingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MailingCity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MailingState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MailingZip = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MailingPhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MailingEmailAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MasterInspectorId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InspectorId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InspectorLicenseNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InspectorLicenseType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InspectorCompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InspectorJobTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InspectorContactName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InspectorAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InspectorCity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InspectorState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InspectorZip = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    InspectorWorkNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InspectorCellNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InspectorFaxNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReasonForInspection = table.Column<int>(type: "int", nullable: false),
                    Compliance1 = table.Column<bool>(type: "bit", nullable: false),
                    Compliance2 = table.Column<bool>(type: "bit", nullable: false),
                    Compliance3 = table.Column<bool>(type: "bit", nullable: false),
                    Compliance4 = table.Column<bool>(type: "bit", nullable: false),
                    Compliance5 = table.Column<bool>(type: "bit", nullable: false),
                    Compliance6 = table.Column<bool>(type: "bit", nullable: false),
                    MaterialServiceLineLead = table.Column<bool>(type: "bit", nullable: false),
                    MaterialServiceLineCopper = table.Column<bool>(type: "bit", nullable: false),
                    MaterialServiceLinePVC = table.Column<bool>(type: "bit", nullable: false),
                    MaterialServiceLineOther = table.Column<bool>(type: "bit", nullable: false),
                    MaterialServiceLineOtherDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MaterialSolderLead = table.Column<bool>(type: "bit", nullable: false),
                    MaterialSolderLeadFree = table.Column<bool>(type: "bit", nullable: false),
                    MaterialSolderSolventWeld = table.Column<bool>(type: "bit", nullable: false),
                    MaterialSolderOther = table.Column<bool>(type: "bit", nullable: false),
                    MaterialSolderOtherDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Disapproved = table.Column<bool>(type: "bit", nullable: false),
                    DisapprovedReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AiOssf = table.Column<bool>(type: "bit", nullable: false),
                    AiWaterWell = table.Column<bool>(type: "bit", nullable: false),
                    AiFireSystem = table.Column<bool>(type: "bit", nullable: false),
                    AiFireSystem2 = table.Column<bool>(type: "bit", nullable: false),
                    AiGreaseTrap = table.Column<bool>(type: "bit", nullable: false),
                    AiSandGrit = table.Column<bool>(type: "bit", nullable: false),
                    AiReclaimedWater = table.Column<bool>(type: "bit", nullable: false),
                    AiIrrigationSystem = table.Column<bool>(type: "bit", nullable: false),
                    AiIrrigationSystem2 = table.Column<bool>(type: "bit", nullable: false),
                    AiHasDomesticPremisesIsolation = table.Column<bool>(type: "bit", nullable: false),
                    AiRequiresDomesticPremisesIsolation = table.Column<bool>(type: "bit", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NeedsValidation = table.Column<bool>(type: "bit", nullable: false),
                    ValidationNewSite = table.Column<bool>(type: "bit", nullable: false),
                    ValidationSiteInformationChanged = table.Column<bool>(type: "bit", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    AmountShare = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    EmailPdf = table.Column<bool>(type: "bit", nullable: false),
                    IncludeWsAccountNumbers = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CsiInspections", x => new { x.WaterSupplierId, x.Id });
                    table.ForeignKey(
                        name: "FK_CsiInspections_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CsiInspections_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CsiInspections_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CsiInspections_Sites_WaterSupplierId_SiteId",
                        columns: x => new { x.WaterSupplierId, x.SiteId },
                        principalTable: "Sites",
                        principalColumns: new[] { "WaterSupplierId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CsiInspections_WaterSuppliers_WaterSupplierId",
                        column: x => x.WaterSupplierId,
                        principalTable: "WaterSuppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CsiInspections_CreatedById",
                table: "CsiInspections",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CsiInspections_DeletedById",
                table: "CsiInspections",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_CsiInspections_UpdatedById",
                table: "CsiInspections",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CsiInspections_WaterSupplierId_SiteId",
                table: "CsiInspections",
                columns: new[] { "WaterSupplierId", "SiteId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "CsiInspections");
        }
    }
}
