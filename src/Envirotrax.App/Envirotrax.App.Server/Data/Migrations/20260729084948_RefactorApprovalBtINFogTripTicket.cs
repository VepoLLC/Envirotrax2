using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorApprovalBtINFogTripTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "FogTripTickets");

            migrationBuilder.AddColumn<int>(
                name: "ApprovedById",
                table: "FogTripTickets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FogTripTickets_WaterSupplierId_ApprovedById",
                table: "FogTripTickets",
                columns: new[] { "WaterSupplierId", "ApprovedById" });

            migrationBuilder.AddForeignKey(
                name: "FK_FogTripTickets_WaterSupplierUsers_WaterSupplierId_ApprovedById",
                table: "FogTripTickets",
                columns: new[] { "WaterSupplierId", "ApprovedById" },
                principalTable: "WaterSupplierUsers",
                principalColumns: new[] { "WaterSupplierId", "UserId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FogTripTickets_WaterSupplierUsers_WaterSupplierId_ApprovedById",
                table: "FogTripTickets");

            migrationBuilder.DropIndex(
                name: "IX_FogTripTickets_WaterSupplierId_ApprovedById",
                table: "FogTripTickets");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "FogTripTickets");

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "FogTripTickets",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
