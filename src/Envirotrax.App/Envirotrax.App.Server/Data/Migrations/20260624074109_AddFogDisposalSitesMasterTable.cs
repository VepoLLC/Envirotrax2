using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFogDisposalSitesMasterTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The existing rows are test data whose WaterSupplierId would become an invalid
            // DisposalSiteId (no matching FogDisposalSites row), violating the new FK. Clear them.
            migrationBuilder.Sql("DELETE FROM [dbo].[FogTransporterDisposalSites];");

            migrationBuilder.DropForeignKey(
                name: "FK_FogTransporterDisposalSites_WaterSuppliers_WaterSupplierId",
                schema: "dbo",
                table: "FogTransporterDisposalSites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FogTransporterDisposalSites",
                schema: "dbo",
                table: "FogTransporterDisposalSites");

            migrationBuilder.RenameColumn(
                name: "WaterSupplierId",
                schema: "dbo",
                table: "FogTransporterDisposalSites",
                newName: "DisposalSiteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FogTransporterDisposalSites",
                schema: "dbo",
                table: "FogTransporterDisposalSites",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "FogDisposalSites",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StateId = table.Column<int>(type: "int", nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    County = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TceqRegion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PermitNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhysicalType = table.Column<int>(type: "int", nullable: false),
                    LocationDescription = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedById = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FogDisposalSites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FogDisposalSites_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FogDisposalSites_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FogDisposalSites_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FogTransporterDisposalSites_DisposalSiteId",
                schema: "dbo",
                table: "FogTransporterDisposalSites",
                column: "DisposalSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_FogDisposalSites_CreatedById",
                schema: "dbo",
                table: "FogDisposalSites",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FogDisposalSites_DeletedById",
                schema: "dbo",
                table: "FogDisposalSites",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_FogDisposalSites_StateId",
                schema: "dbo",
                table: "FogDisposalSites",
                column: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_FogTransporterDisposalSites_FogDisposalSites_DisposalSiteId",
                schema: "dbo",
                table: "FogTransporterDisposalSites",
                column: "DisposalSiteId",
                principalSchema: "dbo",
                principalTable: "FogDisposalSites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FogTransporterDisposalSites_FogDisposalSites_DisposalSiteId",
                schema: "dbo",
                table: "FogTransporterDisposalSites");

            migrationBuilder.DropTable(
                name: "FogDisposalSites",
                schema: "dbo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FogTransporterDisposalSites",
                schema: "dbo",
                table: "FogTransporterDisposalSites");

            migrationBuilder.DropIndex(
                name: "IX_FogTransporterDisposalSites_DisposalSiteId",
                schema: "dbo",
                table: "FogTransporterDisposalSites");

            migrationBuilder.RenameColumn(
                name: "DisposalSiteId",
                schema: "dbo",
                table: "FogTransporterDisposalSites",
                newName: "WaterSupplierId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FogTransporterDisposalSites",
                schema: "dbo",
                table: "FogTransporterDisposalSites",
                columns: new[] { "WaterSupplierId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_FogTransporterDisposalSites_WaterSuppliers_WaterSupplierId",
                schema: "dbo",
                table: "FogTransporterDisposalSites",
                column: "WaterSupplierId",
                principalSchema: "dbo",
                principalTable: "WaterSuppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
