using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class GisAreaBounds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MaxLatitude",
                table: "GisAreas",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MaxLongitude",
                table: "GisAreas",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MinLatitude",
                table: "GisAreas",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MinLongitude",
                table: "GisAreas",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxLatitude",
                table: "GisAreas");

            migrationBuilder.DropColumn(
                name: "MaxLongitude",
                table: "GisAreas");

            migrationBuilder.DropColumn(
                name: "MinLatitude",
                table: "GisAreas");

            migrationBuilder.DropColumn(
                name: "MinLongitude",
                table: "GisAreas");
        }
    }
}
