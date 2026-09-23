using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBackflowTestCompatibilityFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AirGapTestDate",
                table: "BackflowTests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InspectorId",
                table: "BackflowTests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MailingAddress",
                table: "BackflowTests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WaterMeterNumber",
                table: "BackflowTests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BackflowTests_ProfessionalId_InspectorId",
                table: "BackflowTests",
                columns: new[] { "ProfessionalId", "InspectorId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BackflowTests_ProfessionalUsers_ProfessionalId_InspectorId",
                table: "BackflowTests",
                columns: new[] { "ProfessionalId", "InspectorId" },
                principalTable: "ProfessionalUsers",
                principalColumns: new[] { "ProfessionalId", "UserId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BackflowTests_ProfessionalUsers_ProfessionalId_InspectorId",
                table: "BackflowTests");

            migrationBuilder.DropIndex(
                name: "IX_BackflowTests_ProfessionalId_InspectorId",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "AirGapTestDate",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InspectorId",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "MailingAddress",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "WaterMeterNumber",
                table: "BackflowTests");
        }
    }
}
