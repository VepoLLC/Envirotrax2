using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCrmCompanyContacts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyContacts",
                schema: "crm",
                columns: table => new
                {
                    WaterSupplierId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StateId = table.Column<int>(type: "int", nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CellNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyContacts", x => new { x.WaterSupplierId, x.Id });
                    table.ForeignKey(
                        name: "FK_CompanyContacts_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyContacts_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyContacts_Companies_WaterSupplierId_CompanyId",
                        columns: x => new { x.WaterSupplierId, x.CompanyId },
                        principalSchema: "crm",
                        principalTable: "Companies",
                        principalColumns: new[] { "WaterSupplierId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyContacts_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyContacts_WaterSuppliers_WaterSupplierId",
                        column: x => x.WaterSupplierId,
                        principalTable: "WaterSuppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyContacts_CreatedById",
                schema: "crm",
                table: "CompanyContacts",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyContacts_StateId",
                schema: "crm",
                table: "CompanyContacts",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyContacts_UpdatedById",
                schema: "crm",
                table: "CompanyContacts",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyContacts_WaterSupplierId_CompanyId",
                schema: "crm",
                table: "CompanyContacts",
                columns: new[] { "WaterSupplierId", "CompanyId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyContacts",
                schema: "crm");
        }
    }
}
