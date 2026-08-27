using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFogVehiclePermits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FogVehiclePermits_AspNetUsers_DeletedById",
                table: "FogVehiclePermits");

            migrationBuilder.DropForeignKey(
                name: "FK_FogVehiclePermits_AspNetUsers_UpdatedById",
                table: "FogVehiclePermits");

            migrationBuilder.DropIndex(
                name: "IX_FogVehiclePermits_DeletedById",
                table: "FogVehiclePermits");

            migrationBuilder.DropIndex(
                name: "IX_FogVehiclePermits_UpdatedById",
                table: "FogVehiclePermits");

            migrationBuilder.DropIndex(
                name: "IX_FogVehiclePermits_VehicleId",
                table: "FogVehiclePermits");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "FogVehiclePermits");

            migrationBuilder.DropColumn(
                name: "DeletedTime",
                table: "FogVehiclePermits");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "FogVehiclePermits");

            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "FogVehiclePermits");

            migrationBuilder.CreateIndex(
                name: "IX_FogVehiclePermits_VehicleId",
                table: "FogVehiclePermits",
                column: "VehicleId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FogVehiclePermits_VehicleId",
                table: "FogVehiclePermits");

            migrationBuilder.AddColumn<int>(
                name: "DeletedById",
                table: "FogVehiclePermits",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedTime",
                table: "FogVehiclePermits",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedById",
                table: "FogVehiclePermits",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "FogVehiclePermits",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FogVehiclePermits_DeletedById",
                table: "FogVehiclePermits",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_FogVehiclePermits_UpdatedById",
                table: "FogVehiclePermits",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FogVehiclePermits_VehicleId",
                table: "FogVehiclePermits",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_FogVehiclePermits_AspNetUsers_DeletedById",
                table: "FogVehiclePermits",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FogVehiclePermits_AspNetUsers_UpdatedById",
                table: "FogVehiclePermits",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
