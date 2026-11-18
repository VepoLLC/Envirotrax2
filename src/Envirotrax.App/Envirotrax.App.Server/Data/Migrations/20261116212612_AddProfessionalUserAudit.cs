using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessionalUserAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "ProfessionalUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "ProfessionalUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // No real creation date exists for the users already in the table, so they get the date this runs.
            migrationBuilder.Sql("UPDATE ProfessionalUsers SET CreatedTime = GETUTCDATE()");

            migrationBuilder.AddColumn<int>(
                name: "DeletedById",
                table: "ProfessionalUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedTime",
                table: "ProfessionalUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedById",
                table: "ProfessionalUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "ProfessionalUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalUsers_CreatedById",
                table: "ProfessionalUsers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalUsers_DeletedById",
                table: "ProfessionalUsers",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalUsers_UpdatedById",
                table: "ProfessionalUsers",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalUsers_AspNetUsers_CreatedById",
                table: "ProfessionalUsers",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalUsers_AspNetUsers_DeletedById",
                table: "ProfessionalUsers",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalUsers_AspNetUsers_UpdatedById",
                table: "ProfessionalUsers",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalUsers_AspNetUsers_CreatedById",
                table: "ProfessionalUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalUsers_AspNetUsers_DeletedById",
                table: "ProfessionalUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalUsers_AspNetUsers_UpdatedById",
                table: "ProfessionalUsers");

            migrationBuilder.DropIndex(
                name: "IX_ProfessionalUsers_CreatedById",
                table: "ProfessionalUsers");

            migrationBuilder.DropIndex(
                name: "IX_ProfessionalUsers_DeletedById",
                table: "ProfessionalUsers");

            migrationBuilder.DropIndex(
                name: "IX_ProfessionalUsers_UpdatedById",
                table: "ProfessionalUsers");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "ProfessionalUsers");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "ProfessionalUsers");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "ProfessionalUsers");

            migrationBuilder.DropColumn(
                name: "DeletedTime",
                table: "ProfessionalUsers");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "ProfessionalUsers");

            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "ProfessionalUsers");
        }
    }
}
