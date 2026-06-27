using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRainFreezeSensorFieldsToBackflowTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RainFreezeSensorInstalled",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RainFreezeSensorWorkingProperly",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RainFreezeSensorInstalled",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "RainFreezeSensorWorkingProperly",
                table: "BackflowTests");
        }
    }
}
