using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessionalLicenses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LicenseScope",
                table: "ProfessionalLicenseTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "ProfessionalLicenseTypes",
                keyColumn: "IsFireLicense",
                keyValue: true,
                column: "LicenseScope",
                value: 1);

            migrationBuilder.UpdateData(
                table: "ProfessionalLicenseTypes",
                keyColumn: "ProfessionalType",
                keyValue: 5,
                column: "LicenseScope",
                value: 1);

            migrationBuilder.CreateTable(
                name: "ProfessionalLicenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfessionalId = table.Column<int>(type: "int", nullable: false),
                    ProfessionalType = table.Column<int>(type: "int", nullable: false),
                    LicenseTypeId = table.Column<int>(type: "int", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionalLicenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfessionalLicenses_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfessionalLicenses_ProfessionalLicenseTypes_LicenseTypeId",
                        column: x => x.LicenseTypeId,
                        principalTable: "ProfessionalLicenseTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfessionalLicenses_Professionals_ProfessionalId",
                        column: x => x.ProfessionalId,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalLicenses_CreatedById",
                table: "ProfessionalLicenses",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalLicenses_LicenseTypeId",
                table: "ProfessionalLicenses",
                column: "LicenseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalLicenses_ProfessionalId",
                table: "ProfessionalLicenses",
                column: "ProfessionalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfessionalLicenses");

            migrationBuilder.DropColumn(
                name: "LicenseScope",
                table: "ProfessionalLicenseTypes");
        }
    }
}
