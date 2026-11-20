using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWaterSupplierUserAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "WaterSupplierUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "WaterSupplierUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // No real creation date exists for the users already in the table, so they get the date this runs.
            migrationBuilder.Sql("UPDATE WaterSupplierUsers SET CreatedTime = GETUTCDATE()");

            migrationBuilder.AddColumn<int>(
                name: "DeletedById",
                table: "WaterSupplierUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedTime",
                table: "WaterSupplierUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedById",
                table: "WaterSupplierUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "WaterSupplierUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WaterSupplierUsers_CreatedById",
                table: "WaterSupplierUsers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WaterSupplierUsers_DeletedById",
                table: "WaterSupplierUsers",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_WaterSupplierUsers_UpdatedById",
                table: "WaterSupplierUsers",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_WaterSupplierUsers_AspNetUsers_CreatedById",
                table: "WaterSupplierUsers",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WaterSupplierUsers_AspNetUsers_DeletedById",
                table: "WaterSupplierUsers",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WaterSupplierUsers_AspNetUsers_UpdatedById",
                table: "WaterSupplierUsers",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaterSupplierUsers_AspNetUsers_CreatedById",
                table: "WaterSupplierUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_WaterSupplierUsers_AspNetUsers_DeletedById",
                table: "WaterSupplierUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_WaterSupplierUsers_AspNetUsers_UpdatedById",
                table: "WaterSupplierUsers");

            migrationBuilder.DropIndex(
                name: "IX_WaterSupplierUsers_CreatedById",
                table: "WaterSupplierUsers");

            migrationBuilder.DropIndex(
                name: "IX_WaterSupplierUsers_DeletedById",
                table: "WaterSupplierUsers");

            migrationBuilder.DropIndex(
                name: "IX_WaterSupplierUsers_UpdatedById",
                table: "WaterSupplierUsers");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "WaterSupplierUsers");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "WaterSupplierUsers");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "WaterSupplierUsers");

            migrationBuilder.DropColumn(
                name: "DeletedTime",
                table: "WaterSupplierUsers");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "WaterSupplierUsers");

            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "WaterSupplierUsers");
        }
    }
}
