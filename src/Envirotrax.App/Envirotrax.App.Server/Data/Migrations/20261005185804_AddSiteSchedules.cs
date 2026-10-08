using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteSchedules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfessionalId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProfessionalType = table.Column<int>(type: "int", nullable: false),
                    SiteId = table.Column<int>(type: "int", nullable: false),
                    ScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiteSchedules_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SiteSchedules_ProfessionalUsers_ProfessionalId_UserId",
                        columns: x => new { x.ProfessionalId, x.UserId },
                        principalTable: "ProfessionalUsers",
                        principalColumns: new[] { "ProfessionalId", "UserId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SiteSchedules_Professionals_ProfessionalId",
                        column: x => x.ProfessionalId,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SiteSchedules_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SiteSchedules_CreatedById",
                table: "SiteSchedules",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SiteSchedules_ProfessionalId_UserId",
                table: "SiteSchedules",
                columns: new[] { "ProfessionalId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_SiteSchedules_SiteId_ProfessionalId_UserId_ProfessionalType",
                table: "SiteSchedules",
                columns: new[] { "SiteId", "ProfessionalId", "UserId", "ProfessionalType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteSchedules");
        }
    }
}
