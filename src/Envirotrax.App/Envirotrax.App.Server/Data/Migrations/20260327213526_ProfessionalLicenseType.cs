using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProfessionalLicenseType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LicenseTypeId",
                table: "ProfessionalUserLicenses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalUserLicenses_LicenseTypeId",
                table: "ProfessionalUserLicenses",
                column: "LicenseTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalUserLicenses_ProfessionalLicenseTypes_LicenseTypeId",
                table: "ProfessionalUserLicenses",
                column: "LicenseTypeId",
                principalTable: "ProfessionalLicenseTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalUserLicenses_ProfessionalLicenseTypes_LicenseTypeId",
                table: "ProfessionalUserLicenses");

            migrationBuilder.DropIndex(
                name: "IX_ProfessionalUserLicenses_LicenseTypeId",
                table: "ProfessionalUserLicenses");

            migrationBuilder.DropColumn(
                name: "LicenseTypeId",
                table: "ProfessionalUserLicenses");
        }
    }
}
