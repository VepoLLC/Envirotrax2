using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGisAreas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "GisCenterLatitude",
                table: "WaterSuppliers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GisCenterLongitude",
                table: "WaterSuppliers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GisCenterZoom",
                table: "WaterSuppliers",
                type: "float",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GisAreas",
                columns: table => new
                {
                    WaterSupplierId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedById = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GisAreas", x => new { x.WaterSupplierId, x.Id });
                    table.ForeignKey(
                        name: "FK_GisAreas_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GisAreas_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GisAreas_WaterSuppliers_WaterSupplierId",
                        column: x => x.WaterSupplierId,
                        principalTable: "WaterSuppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GisAreaCoordinates",
                columns: table => new
                {
                    WaterSupplierId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitde = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GisAreaCoordinates", x => new { x.WaterSupplierId, x.Id });
                    table.ForeignKey(
                        name: "FK_GisAreaCoordinates_GisAreas_WaterSupplierId_AreaId",
                        columns: x => new { x.WaterSupplierId, x.AreaId },
                        principalTable: "GisAreas",
                        principalColumns: new[] { "WaterSupplierId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GisAreaCoordinates_WaterSuppliers_WaterSupplierId",
                        column: x => x.WaterSupplierId,
                        principalTable: "WaterSuppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GisAreaCoordinates_WaterSupplierId_AreaId",
                table: "GisAreaCoordinates",
                columns: new[] { "WaterSupplierId", "AreaId" });

            migrationBuilder.CreateIndex(
                name: "IX_GisAreas_CreatedById",
                table: "GisAreas",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_GisAreas_DeletedById",
                table: "GisAreas",
                column: "DeletedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GisAreaCoordinates");

            migrationBuilder.DropTable(
                name: "GisAreas");

            migrationBuilder.DropColumn(
                name: "GisCenterLatitude",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "GisCenterLongitude",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "GisCenterZoom",
                table: "WaterSuppliers");
        }
    }
}
