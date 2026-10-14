using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenewalEmailVerificationSiteScoped : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RenewalEmailVerifications_WaterSuppliers_WaterSupplierId",
                table: "RenewalEmailVerifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RenewalEmailVerifications",
                table: "RenewalEmailVerifications");

            migrationBuilder.DropIndex(
                name: "IX_RenewalEmailVerifications_SiteId_IsVerified_CreatedDate",
                table: "RenewalEmailVerifications");

            migrationBuilder.DropColumn(
                name: "WaterSupplierId",
                table: "RenewalEmailVerifications");

            migrationBuilder.AlterColumn<string>(
                name: "MailingEmailAddress",
                table: "Sites",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MailingEmailAddress",
                table: "FogInspections",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MailingEmailAddress",
                table: "CsiInspections",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MailingEmailAddress",
                table: "BackflowTests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RenewalEmailVerifications",
                table: "RenewalEmailVerifications",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RenewalEmailVerifications_SiteId",
                table: "RenewalEmailVerifications",
                column: "SiteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RenewalEmailVerifications",
                table: "RenewalEmailVerifications");

            migrationBuilder.DropIndex(
                name: "IX_RenewalEmailVerifications_SiteId",
                table: "RenewalEmailVerifications");

            migrationBuilder.AlterColumn<string>(
                name: "MailingEmailAddress",
                table: "Sites",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WaterSupplierId",
                table: "RenewalEmailVerifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "MailingEmailAddress",
                table: "FogInspections",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MailingEmailAddress",
                table: "CsiInspections",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MailingEmailAddress",
                table: "BackflowTests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RenewalEmailVerifications",
                table: "RenewalEmailVerifications",
                columns: new[] { "WaterSupplierId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_RenewalEmailVerifications_SiteId_IsVerified_CreatedDate",
                table: "RenewalEmailVerifications",
                columns: new[] { "SiteId", "IsVerified", "CreatedDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_RenewalEmailVerifications_WaterSuppliers_WaterSupplierId",
                table: "RenewalEmailVerifications",
                column: "WaterSupplierId",
                principalTable: "WaterSuppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
