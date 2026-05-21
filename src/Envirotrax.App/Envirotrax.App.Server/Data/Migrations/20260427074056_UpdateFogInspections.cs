using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFogInspections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MasterInspectorId",
                table: "FogInspections");

            migrationBuilder.DropColumn(
                name: "ValidatedBy",
                table: "FogInspections");

            migrationBuilder.DropColumn(
                name: "ValidationClearedDate",
                table: "FogInspections");

            migrationBuilder.DropColumn(
                name: "ValidationLockedDate",
                table: "FogInspections");

            migrationBuilder.DropColumn(
                name: "ValidationNotes",
                table: "FogInspections");

            migrationBuilder.DropColumn(
                name: "ValidationOnHold",
                table: "FogInspections");

            migrationBuilder.AlterColumn<int>(
                name: "InspectorId",
                table: "FogInspections",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProfessionalId",
                table: "FogInspections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FogInspections_ProfessionalId_InspectorId",
                table: "FogInspections",
                columns: new[] { "ProfessionalId", "InspectorId" });

            migrationBuilder.AddForeignKey(
                name: "FK_FogInspections_ProfessionalUsers_ProfessionalId_InspectorId",
                table: "FogInspections",
                columns: new[] { "ProfessionalId", "InspectorId" },
                principalTable: "ProfessionalUsers",
                principalColumns: new[] { "ProfessionalId", "UserId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FogInspections_Professionals_ProfessionalId",
                table: "FogInspections",
                column: "ProfessionalId",
                principalTable: "Professionals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FogInspections_ProfessionalUsers_ProfessionalId_InspectorId",
                table: "FogInspections");

            migrationBuilder.DropForeignKey(
                name: "FK_FogInspections_Professionals_ProfessionalId",
                table: "FogInspections");

            migrationBuilder.DropIndex(
                name: "IX_FogInspections_ProfessionalId_InspectorId",
                table: "FogInspections");

            migrationBuilder.DropColumn(
                name: "ProfessionalId",
                table: "FogInspections");

            migrationBuilder.AlterColumn<string>(
                name: "InspectorId",
                table: "FogInspections",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "MasterInspectorId",
                table: "FogInspections",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValidatedBy",
                table: "FogInspections",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidationClearedDate",
                table: "FogInspections",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidationLockedDate",
                table: "FogInspections",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValidationNotes",
                table: "FogInspections",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ValidationOnHold",
                table: "FogInspections",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
