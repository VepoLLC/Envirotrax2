using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCsiInspectionAssembliesAndRecordLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CsiInspectionVisuallyIdentifiedAssemblies",
                columns: table => new
                {
                    WaterSupplierId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InspectionId = table.Column<int>(type: "int", nullable: false),
                    TestId = table.Column<int>(type: "int", nullable: true),
                    SubmissionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VisuallyIdentified = table.Column<bool>(type: "bit", nullable: false),
                    DeviceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AssemblyDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AssemblyDescription2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SerialNumber2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HazardType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HazardTypeOtherDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LocationDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    TestResult = table.Column<int>(type: "int", nullable: false),
                    OutOfService = table.Column<bool>(type: "bit", nullable: false),
                    TestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CsiInspectionVisuallyIdentifiedAssemblies", x => new { x.WaterSupplierId, x.Id });
                    table.ForeignKey(
                        name: "FK_CsiInspectionVisuallyIdentifiedAssemblies_BackflowTests_WaterSupplierId_TestId",
                        columns: x => new { x.WaterSupplierId, x.TestId },
                        principalTable: "BackflowTests",
                        principalColumns: new[] { "WaterSupplierId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CsiInspectionVisuallyIdentifiedAssemblies_CsiInspections_WaterSupplierId_InspectionId",
                        columns: x => new { x.WaterSupplierId, x.InspectionId },
                        principalTable: "CsiInspections",
                        principalColumns: new[] { "WaterSupplierId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CsiInspectionVisuallyIdentifiedAssemblies_WaterSuppliers_WaterSupplierId",
                        column: x => x.WaterSupplierId,
                        principalTable: "WaterSuppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecordLogs",
                columns: table => new
                {
                    WaterSupplierId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LogType = table.Column<int>(type: "int", nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RecordId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordLogs", x => new { x.WaterSupplierId, x.Id });
                    table.ForeignKey(
                        name: "FK_RecordLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordLogs_WaterSuppliers_WaterSupplierId",
                        column: x => x.WaterSupplierId,
                        principalTable: "WaterSuppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CsiInspectionVisuallyIdentifiedAssemblies_WaterSupplierId_InspectionId",
                table: "CsiInspectionVisuallyIdentifiedAssemblies",
                columns: new[] { "WaterSupplierId", "InspectionId" });

            migrationBuilder.CreateIndex(
                name: "IX_CsiInspectionVisuallyIdentifiedAssemblies_WaterSupplierId_TestId",
                table: "CsiInspectionVisuallyIdentifiedAssemblies",
                columns: new[] { "WaterSupplierId", "TestId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecordLogs_UserId",
                table: "RecordLogs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CsiInspectionVisuallyIdentifiedAssemblies");

            migrationBuilder.DropTable(
                name: "RecordLogs");
        }
    }
}
