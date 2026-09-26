using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddApiAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApiKeyHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    WaterSupplierId = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsLive = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PermissionWaterSuppliers = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    PermissionSites = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    PermissionBackflowTests = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    PermissionCsiInspections = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    PermissionFogInspections = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    PermissionFogTripTickets = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiAccounts_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiAccounts_CreatedById",
                table: "ApiAccounts",
                column: "CreatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiAccounts");
        }
    }
}
