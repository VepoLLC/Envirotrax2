using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProfessionalUserAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "ProfessionalUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBackflowTester",
                table: "ProfessionalUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCsiInspector",
                table: "ProfessionalUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFogInspector",
                table: "ProfessionalUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFogTransporter",
                table: "ProfessionalUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWiseGuy",
                table: "ProfessionalUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "ProfessionalUsers");

            migrationBuilder.DropColumn(
                name: "IsBackflowTester",
                table: "ProfessionalUsers");

            migrationBuilder.DropColumn(
                name: "IsCsiInspector",
                table: "ProfessionalUsers");

            migrationBuilder.DropColumn(
                name: "IsFogInspector",
                table: "ProfessionalUsers");

            migrationBuilder.DropColumn(
                name: "IsFogTransporter",
                table: "ProfessionalUsers");

            migrationBuilder.DropColumn(
                name: "IsWiseGuy",
                table: "ProfessionalUsers");
        }
    }
}
