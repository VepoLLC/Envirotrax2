using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationDigestScheduling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailBatchId",
                table: "Notifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotificationSettingId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_WaterSupplierId_NotificationSettingId_CreatedTime",
                table: "Notifications",
                columns: new[] { "WaterSupplierId", "NotificationSettingId", "CreatedTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Notifications_WaterSupplierId_NotificationSettingId_CreatedTime",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "EmailBatchId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NotificationSettingId",
                table: "Notifications");
        }
    }
}
