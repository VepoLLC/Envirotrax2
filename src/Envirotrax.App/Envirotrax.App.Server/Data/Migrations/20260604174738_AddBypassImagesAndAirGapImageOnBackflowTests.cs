using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBypassImagesAndAirGapImageOnBackflowTests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AirGapImagePath",
                table: "BackflowTests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BypassAssemblyImagePath",
                table: "BackflowTests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BypassSerialNumberImagePath",
                table: "BackflowTests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AirGapImagePath",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "BypassAssemblyImagePath",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "BypassSerialNumberImagePath",
                table: "BackflowTests");
        }
    }
}
