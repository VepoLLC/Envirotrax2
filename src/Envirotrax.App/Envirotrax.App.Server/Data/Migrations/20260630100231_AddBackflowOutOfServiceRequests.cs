using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBackflowOutOfServiceRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BackflowOutOfServiceRequests",
                schema: "dbo",
                columns: table => new
                {
                    WaterSupplierId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfessionalId = table.Column<int>(type: "int", nullable: false),
                    BpatId = table.Column<int>(type: "int", nullable: true),
                    TestId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReplacementAssemblyTestId = table.Column<int>(type: "int", nullable: true),
                    OutOfServiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClearedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackflowOutOfServiceRequests", x => new { x.WaterSupplierId, x.Id });
                    table.ForeignKey(
                        name: "FK_BackflowOutOfServiceRequests_BackflowTests_WaterSupplierId_ReplacementAssemblyTestId",
                        columns: x => new { x.WaterSupplierId, x.ReplacementAssemblyTestId },
                        principalSchema: "dbo",
                        principalTable: "BackflowTests",
                        principalColumns: new[] { "WaterSupplierId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BackflowOutOfServiceRequests_BackflowTests_WaterSupplierId_TestId",
                        columns: x => new { x.WaterSupplierId, x.TestId },
                        principalSchema: "dbo",
                        principalTable: "BackflowTests",
                        principalColumns: new[] { "WaterSupplierId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BackflowOutOfServiceRequests_ProfessionalUsers_ProfessionalId_BpatId",
                        columns: x => new { x.ProfessionalId, x.BpatId },
                        principalSchema: "dbo",
                        principalTable: "ProfessionalUsers",
                        principalColumns: new[] { "ProfessionalId", "UserId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BackflowOutOfServiceRequests_Professionals_ProfessionalId",
                        column: x => x.ProfessionalId,
                        principalSchema: "dbo",
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BackflowOutOfServiceRequests_WaterSuppliers_WaterSupplierId",
                        column: x => x.WaterSupplierId,
                        principalSchema: "dbo",
                        principalTable: "WaterSuppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BackflowOutOfServiceRequests_ProfessionalId_BpatId",
                schema: "dbo",
                table: "BackflowOutOfServiceRequests",
                columns: new[] { "ProfessionalId", "BpatId" });

            migrationBuilder.CreateIndex(
                name: "IX_BackflowOutOfServiceRequests_WaterSupplierId_ReplacementAssemblyTestId",
                schema: "dbo",
                table: "BackflowOutOfServiceRequests",
                columns: new[] { "WaterSupplierId", "ReplacementAssemblyTestId" });

            migrationBuilder.CreateIndex(
                name: "IX_BackflowOutOfServiceRequests_WaterSupplierId_TestId",
                schema: "dbo",
                table: "BackflowOutOfServiceRequests",
                columns: new[] { "WaterSupplierId", "TestId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BackflowOutOfServiceRequests",
                schema: "dbo");
        }
    }
}
