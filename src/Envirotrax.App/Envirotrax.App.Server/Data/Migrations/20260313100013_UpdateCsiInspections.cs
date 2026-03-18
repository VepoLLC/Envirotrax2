using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCsiInspections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncludeWsAccountNumbers",
                table: "CsiInspections");

            migrationBuilder.DropColumn(
                name: "MailingState",
                table: "CsiInspections");

            migrationBuilder.DropColumn(
                name: "PropertyState",
                table: "CsiInspections");

            migrationBuilder.AlterColumn<int>(
                name: "SiteId",
                table: "CsiInspections",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "InspectionDate",
                table: "CsiInspections",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MailingStateId",
                table: "CsiInspections",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PropertyStateId",
                table: "CsiInspections",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CsiInspections_MailingStateId",
                table: "CsiInspections",
                column: "MailingStateId");

            migrationBuilder.CreateIndex(
                name: "IX_CsiInspections_PropertyStateId",
                table: "CsiInspections",
                column: "PropertyStateId");

            migrationBuilder.AddForeignKey(
                name: "FK_CsiInspections_States_MailingStateId",
                table: "CsiInspections",
                column: "MailingStateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CsiInspections_States_PropertyStateId",
                table: "CsiInspections",
                column: "PropertyStateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CsiInspections_States_MailingStateId",
                table: "CsiInspections");

            migrationBuilder.DropForeignKey(
                name: "FK_CsiInspections_States_PropertyStateId",
                table: "CsiInspections");

            migrationBuilder.DropIndex(
                name: "IX_CsiInspections_MailingStateId",
                table: "CsiInspections");

            migrationBuilder.DropIndex(
                name: "IX_CsiInspections_PropertyStateId",
                table: "CsiInspections");

            migrationBuilder.DropColumn(
                name: "MailingStateId",
                table: "CsiInspections");

            migrationBuilder.DropColumn(
                name: "PropertyStateId",
                table: "CsiInspections");

            migrationBuilder.AlterColumn<int>(
                name: "SiteId",
                table: "CsiInspections",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InspectionDate",
                table: "CsiInspections",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IncludeWsAccountNumbers",
                table: "CsiInspections",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MailingState",
                table: "CsiInspections",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertyState",
                table: "CsiInspections",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
