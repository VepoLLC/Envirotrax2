using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSubmissionValidationFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ValidationOnHold",
                table: "FogInspections",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ValidationOnHold",
                table: "CsiInspections",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ValidationCreatedFromCsi",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ValidationDeviceMoved",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ValidationHazardTypeChanged",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ValidationOnHold",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidationOnHold",
                table: "FogInspections");

            migrationBuilder.DropColumn(
                name: "ValidationOnHold",
                table: "CsiInspections");

            migrationBuilder.DropColumn(
                name: "ValidationCreatedFromCsi",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "ValidationDeviceMoved",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "ValidationHazardTypeChanged",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "ValidationOnHold",
                table: "BackflowTests");
        }
    }
}
