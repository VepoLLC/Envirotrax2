using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsOnBackflowTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BackflowScheduleMonth",
                table: "BackflowTests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ForceRenewal",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ForceRenewalYears",
                table: "BackflowTests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackflowScheduleMonth",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "ForceRenewal",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "ForceRenewalYears",
                table: "BackflowTests");
        }
    }
}
