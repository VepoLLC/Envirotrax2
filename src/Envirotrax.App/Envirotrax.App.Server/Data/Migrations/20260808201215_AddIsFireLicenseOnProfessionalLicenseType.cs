using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFireLicenseOnProfessionalLicenseType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFireLicense",
                table: "ProfessionalLicenseTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE ProfessionalLicenseTypes SET IsFireLicense = 1 WHERE Name IN ('TX Fire Marshal Office - SCR', 'ASSE - Fire BP Tester')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFireLicense",
                table: "ProfessionalLicenseTypes");
        }
    }
}
