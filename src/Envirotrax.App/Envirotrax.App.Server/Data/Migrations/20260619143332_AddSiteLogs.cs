using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteLogs",
                columns: table => new
                {
                    WaterSupplierId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteId = table.Column<int>(type: "int", nullable: false),
                    LogType = table.Column<int>(type: "int", nullable: false),
                    NoteText = table.Column<string>(type: "text", nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssemblyId = table.Column<int>(type: "int", nullable: true),
                    FileAttachmentName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FileAttachmentPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SkipFile = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteLogs", x => new { x.WaterSupplierId, x.Id });
                    table.ForeignKey(
                        name: "FK_SiteLogs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SiteLogs_BackflowTests_WaterSupplierId_AssemblyId",
                        columns: x => new { x.WaterSupplierId, x.AssemblyId },
                        principalTable: "BackflowTests",
                        principalColumns: new[] { "WaterSupplierId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SiteLogs_Sites_WaterSupplierId_SiteId",
                        columns: x => new { x.WaterSupplierId, x.SiteId },
                        principalTable: "Sites",
                        principalColumns: new[] { "WaterSupplierId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SiteLogs_WaterSuppliers_WaterSupplierId",
                        column: x => x.WaterSupplierId,
                        principalTable: "WaterSuppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SiteLogs_CreatedById",
                table: "SiteLogs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SiteLogs_WaterSupplierId_AssemblyId",
                table: "SiteLogs",
                columns: new[] { "WaterSupplierId", "AssemblyId" });

            migrationBuilder.CreateIndex(
                name: "IX_SiteLogs_WaterSupplierId_SiteId",
                table: "SiteLogs",
                columns: new[] { "WaterSupplierId", "SiteId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteLogs");
        }
    }
}
