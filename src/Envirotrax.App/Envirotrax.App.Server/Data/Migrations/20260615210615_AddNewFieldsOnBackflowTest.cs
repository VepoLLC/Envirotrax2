using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsOnBackflowTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RepairRV2",
                table: "BackflowTests",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RepairRV",
                table: "BackflowTests",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalCV2HeldPSID2",
                table: "BackflowTests",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepairPvbAirInlet",
                table: "BackflowTests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepairPvbAirInletDetails",
                table: "BackflowTests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepairPvbCV",
                table: "BackflowTests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepairPvbCVDetails",
                table: "BackflowTests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalCV2HeldPSID2",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "RepairPvbAirInlet",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "RepairPvbAirInletDetails",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "RepairPvbCV",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "RepairPvbCVDetails",
                table: "BackflowTests");

            migrationBuilder.AlterColumn<string>(
                name: "RepairRV2",
                table: "BackflowTests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RepairRV",
                table: "BackflowTests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);
        }
    }
}
