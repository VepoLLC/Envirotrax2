using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProfessionalServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasBackflowTesting",
                table: "Professionals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasCsiInspection",
                table: "Professionals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasFogInspection",
                table: "Professionals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasFogTransportation",
                table: "Professionals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasWiseGuys",
                table: "Professionals",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasBackflowTesting",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "HasCsiInspection",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "HasFogInspection",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "HasFogTransportation",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "HasWiseGuys",
                table: "Professionals");
        }
    }
}
