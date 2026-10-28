using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLetterGeneratorSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NonCompliantPrint1",
                table: "CsiSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NonCompliantPrint2",
                table: "CsiSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "NoticeImageFitSize",
                table: "CsiSettings",
                type: "decimal(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NoticeImageFitType",
                table: "CsiSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "NonCompliantPrint1",
                table: "BackflowSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NonCompliantPrint2",
                table: "BackflowSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "NoticeImageFitSize",
                table: "BackflowSettings",
                type: "decimal(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NoticeImageFitType",
                table: "BackflowSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UnknownAssembliesLettersBackgroundColor",
                table: "BackflowSettings",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "#d3d3d3");

            migrationBuilder.AddColumn<string>(
                name: "UnknownAssembliesLettersBorderColor",
                table: "BackflowSettings",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "#000000");

            migrationBuilder.AddColumn<string>(
                name: "UnknownAssembliesLettersForegroundColor",
                table: "BackflowSettings",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "#000000");

            migrationBuilder.AddColumn<string>(
                name: "UnknownAssembliesMessage",
                table: "BackflowSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UnknownAssembliesNotice",
                table: "BackflowSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UnknownAssembliesTitle",
                table: "BackflowSettings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NonCompliantPrint1",
                table: "CsiSettings");

            migrationBuilder.DropColumn(
                name: "NonCompliantPrint2",
                table: "CsiSettings");

            migrationBuilder.DropColumn(
                name: "NoticeImageFitSize",
                table: "CsiSettings");

            migrationBuilder.DropColumn(
                name: "NoticeImageFitType",
                table: "CsiSettings");

            migrationBuilder.DropColumn(
                name: "NonCompliantPrint1",
                table: "BackflowSettings");

            migrationBuilder.DropColumn(
                name: "NonCompliantPrint2",
                table: "BackflowSettings");

            migrationBuilder.DropColumn(
                name: "NoticeImageFitSize",
                table: "BackflowSettings");

            migrationBuilder.DropColumn(
                name: "NoticeImageFitType",
                table: "BackflowSettings");

            migrationBuilder.DropColumn(
                name: "UnknownAssembliesLettersBackgroundColor",
                table: "BackflowSettings");

            migrationBuilder.DropColumn(
                name: "UnknownAssembliesLettersBorderColor",
                table: "BackflowSettings");

            migrationBuilder.DropColumn(
                name: "UnknownAssembliesLettersForegroundColor",
                table: "BackflowSettings");

            migrationBuilder.DropColumn(
                name: "UnknownAssembliesMessage",
                table: "BackflowSettings");

            migrationBuilder.DropColumn(
                name: "UnknownAssembliesNotice",
                table: "BackflowSettings");

            migrationBuilder.DropColumn(
                name: "UnknownAssembliesTitle",
                table: "BackflowSettings");
        }
    }
}
