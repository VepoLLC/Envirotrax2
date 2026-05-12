using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLetterMessageSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImpendingMessage",
                table: "CsiSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImpendingTitle",
                table: "CsiSettings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NonCompliantMessage",
                table: "CsiSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NonCompliantTitle",
                table: "CsiSettings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NoticeBodyFont",
                table: "CsiSettings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoticeBodyFontSize",
                table: "CsiSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PastDueMessage",
                table: "CsiSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PastDueTitle",
                table: "CsiSettings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExpiredMessage",
                table: "BackflowSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExpiredTitle",
                table: "BackflowSettings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExpiringMessage",
                table: "BackflowSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExpiringTitle",
                table: "BackflowSettings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NonCompliantMessage",
                table: "BackflowSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NonCompliantTitle",
                table: "BackflowSettings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NoticeBodyFont",
                table: "BackflowSettings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoticeBodyFontSize",
                table: "BackflowSettings",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImpendingMessage",
                table: "CsiSettings");

            migrationBuilder.DropColumn(
                name: "ImpendingTitle",
                table: "CsiSettings");

            migrationBuilder.DropColumn(
                name: "NonCompliantMessage",
                table: "CsiSettings");

            migrationBuilder.DropColumn(
                name: "NonCompliantTitle",
                table: "CsiSettings");

            migrationBuilder.DropColumn(
                name: "NoticeBodyFont",
                table: "CsiSettings");

            migrationBuilder.DropColumn(
                name: "NoticeBodyFontSize",
                table: "CsiSettings");

            migrationBuilder.DropColumn(
                name: "PastDueMessage",
                table: "CsiSettings");

            migrationBuilder.DropColumn(
                name: "PastDueTitle",
                table: "CsiSettings");

            migrationBuilder.DropColumn(
                name: "ExpiredMessage",
                table: "BackflowSettings");

            migrationBuilder.DropColumn(
                name: "ExpiredTitle",
                table: "BackflowSettings");

            migrationBuilder.DropColumn(
                name: "ExpiringMessage",
                table: "BackflowSettings");

            migrationBuilder.DropColumn(
                name: "ExpiringTitle",
                table: "BackflowSettings");

            migrationBuilder.DropColumn(
                name: "NonCompliantMessage",
                table: "BackflowSettings");

            migrationBuilder.DropColumn(
                name: "NonCompliantTitle",
                table: "BackflowSettings");

            migrationBuilder.DropColumn(
                name: "NoticeBodyFont",
                table: "BackflowSettings");

            migrationBuilder.DropColumn(
                name: "NoticeBodyFontSize",
                table: "BackflowSettings");
        }
    }
}
