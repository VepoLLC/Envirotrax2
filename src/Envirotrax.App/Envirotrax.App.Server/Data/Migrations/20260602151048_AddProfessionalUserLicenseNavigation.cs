using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessionalUserLicenseNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalUserLicenses_ProfessionalUsers_ProfessionalId_UserId",
                table: "ProfessionalUserLicenses");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalUserLicenses_ProfessionalUsers_ProfessionalId_UserId",
                table: "ProfessionalUserLicenses",
                columns: new[] { "ProfessionalId", "UserId" },
                principalTable: "ProfessionalUsers",
                principalColumns: new[] { "ProfessionalId", "UserId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalUserLicenses_ProfessionalUsers_ProfessionalId_UserId",
                table: "ProfessionalUserLicenses");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalUserLicenses_ProfessionalUsers_ProfessionalId_UserId",
                table: "ProfessionalUserLicenses",
                columns: new[] { "ProfessionalId", "UserId" },
                principalTable: "ProfessionalUsers",
                principalColumns: new[] { "ProfessionalId", "UserId" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
