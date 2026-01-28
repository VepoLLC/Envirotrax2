using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewFieldsInProfessionals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactName",
                table: "ProfessionalUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "ProfessionalUsers",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Professionals",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Professionals",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FaxNumber",
                table: "Professionals",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HidePublicListing",
                table: "Professionals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Professionals",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebSiteUrl",
                table: "Professionals",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "Professionals",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactName",
                table: "ProfessionalUsers");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "ProfessionalUsers");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "FaxNumber",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "HidePublicListing",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "WebSiteUrl",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "Professionals");
        }
    }
}
