using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBackflowTests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "BackflowTests",
                schema: "dbo",
                columns: table => new
                {
                    WaterSupplierId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteId = table.Column<int>(type: "int", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmissionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    JobNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MasterBpatId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BpatId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BpatLicenseNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BpatLicenseExpiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BpatCompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BpatContactName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BpatAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BpatCity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BpatState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BpatZip = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BpatWorkNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BpatCellNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
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
                    DeviceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UnknownSerialNumber = table.Column<bool>(type: "bit", nullable: false),
                    LocationDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HazardType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HazardTypeOtherDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReasonForTest = table.Column<int>(type: "int", nullable: false),
                    InstallationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InitialTestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RepairTestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinalTestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TestResult = table.Column<int>(type: "int", nullable: false),
                    ProperlyInstalled = table.Column<bool>(type: "bit", nullable: false),
                    NonPotable = table.Column<bool>(type: "bit", nullable: false),
                    GaugeManufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GaugeModel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GaugeSerialNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GaugeLastCalibrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GaugeNonPotable = table.Column<bool>(type: "bit", nullable: false),
                    MeterNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PermitNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Ossf = table.Column<bool>(type: "bit", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    Disapproved = table.Column<bool>(type: "bit", nullable: false),
                    OutOfService = table.Column<bool>(type: "bit", nullable: false),
                    OutOfServiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    AmountShare = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    EmailPdf = table.Column<bool>(type: "bit", nullable: false),
                    Rejected = table.Column<bool>(type: "bit", nullable: false),
                    RejectedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RejectedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NeedsValidation = table.Column<bool>(type: "bit", nullable: false),
                    ValidationNewSite = table.Column<bool>(type: "bit", nullable: false),
                    ValidationSiteInformationChanged = table.Column<bool>(type: "bit", nullable: false),
                    ValidationUnknownSerialNumber = table.Column<bool>(type: "bit", nullable: false),
                    ValidationDeviceInformationChanged = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackflowTests", x => new { x.WaterSupplierId, x.Id });
                    table.ForeignKey(
                        name: "FK_BackflowTests_Sites_WaterSupplierId_SiteId",
                        columns: x => new { x.WaterSupplierId, x.SiteId },
                        principalTable: "Sites",
                        principalColumns: new[] { "WaterSupplierId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BackflowTests_WaterSuppliers_WaterSupplierId",
                        column: x => x.WaterSupplierId,
                        principalTable: "WaterSuppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BackflowTests_WaterSupplierId_SiteId",
                schema: "dbo",
                table: "BackflowTests",
                columns: new[] { "WaterSupplierId", "SiteId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BackflowTests",
                schema: "dbo");
        }
    }
}
