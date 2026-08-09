using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveWaterSupplierIdFromSitePrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BackflowTests_Sites_WaterSupplierId_SiteId",
                table: "BackflowTests");

            migrationBuilder.DropForeignKey(
                name: "FK_CsiInspections_Sites_WaterSupplierId_SiteId",
                table: "CsiInspections");

            migrationBuilder.DropForeignKey(
                name: "FK_FogInspections_Sites_WaterSupplierId_SiteId",
                table: "FogInspections");

            migrationBuilder.DropForeignKey(
                name: "FK_FogTripTickets_Sites_WaterSupplierId_SiteId",
                table: "FogTripTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_SiteLogs_Sites_WaterSupplierId_SiteId",
                table: "SiteLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sites",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_SiteLogs_WaterSupplierId_SiteId",
                table: "SiteLogs");

            migrationBuilder.DropIndex(
                name: "IX_FogTripTickets_WaterSupplierId_SiteId",
                table: "FogTripTickets");

            migrationBuilder.DropIndex(
                name: "IX_FogInspections_WaterSupplierId_SiteId",
                table: "FogInspections");

            migrationBuilder.DropIndex(
                name: "IX_CsiInspections_WaterSupplierId_SiteId",
                table: "CsiInspections");

            migrationBuilder.DropIndex(
                name: "IX_BackflowTests_WaterSupplierId_SiteId",
                table: "BackflowTests");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sites",
                table: "Sites",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SiteLogs_SiteId",
                table: "SiteLogs",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_FogTripTickets_SiteId",
                table: "FogTripTickets",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_FogInspections_SiteId",
                table: "FogInspections",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_CsiInspections_SiteId",
                table: "CsiInspections",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_BackflowTests_SiteId",
                table: "BackflowTests",
                column: "SiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_BackflowTests_Sites_SiteId",
                table: "BackflowTests",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CsiInspections_Sites_SiteId",
                table: "CsiInspections",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FogInspections_Sites_SiteId",
                table: "FogInspections",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FogTripTickets_Sites_SiteId",
                table: "FogTripTickets",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SiteLogs_Sites_SiteId",
                table: "SiteLogs",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BackflowTests_Sites_SiteId",
                table: "BackflowTests");

            migrationBuilder.DropForeignKey(
                name: "FK_CsiInspections_Sites_SiteId",
                table: "CsiInspections");

            migrationBuilder.DropForeignKey(
                name: "FK_FogInspections_Sites_SiteId",
                table: "FogInspections");

            migrationBuilder.DropForeignKey(
                name: "FK_FogTripTickets_Sites_SiteId",
                table: "FogTripTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_SiteLogs_Sites_SiteId",
                table: "SiteLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sites",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_SiteLogs_SiteId",
                table: "SiteLogs");

            migrationBuilder.DropIndex(
                name: "IX_FogTripTickets_SiteId",
                table: "FogTripTickets");

            migrationBuilder.DropIndex(
                name: "IX_FogInspections_SiteId",
                table: "FogInspections");

            migrationBuilder.DropIndex(
                name: "IX_CsiInspections_SiteId",
                table: "CsiInspections");

            migrationBuilder.DropIndex(
                name: "IX_BackflowTests_SiteId",
                table: "BackflowTests");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sites",
                table: "Sites",
                columns: new[] { "WaterSupplierId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_SiteLogs_WaterSupplierId_SiteId",
                table: "SiteLogs",
                columns: new[] { "WaterSupplierId", "SiteId" });

            migrationBuilder.CreateIndex(
                name: "IX_FogTripTickets_WaterSupplierId_SiteId",
                table: "FogTripTickets",
                columns: new[] { "WaterSupplierId", "SiteId" });

            migrationBuilder.CreateIndex(
                name: "IX_FogInspections_WaterSupplierId_SiteId",
                table: "FogInspections",
                columns: new[] { "WaterSupplierId", "SiteId" });

            migrationBuilder.CreateIndex(
                name: "IX_CsiInspections_WaterSupplierId_SiteId",
                table: "CsiInspections",
                columns: new[] { "WaterSupplierId", "SiteId" });

            migrationBuilder.CreateIndex(
                name: "IX_BackflowTests_WaterSupplierId_SiteId",
                table: "BackflowTests",
                columns: new[] { "WaterSupplierId", "SiteId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BackflowTests_Sites_WaterSupplierId_SiteId",
                table: "BackflowTests",
                columns: new[] { "WaterSupplierId", "SiteId" },
                principalTable: "Sites",
                principalColumns: new[] { "WaterSupplierId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CsiInspections_Sites_WaterSupplierId_SiteId",
                table: "CsiInspections",
                columns: new[] { "WaterSupplierId", "SiteId" },
                principalTable: "Sites",
                principalColumns: new[] { "WaterSupplierId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FogInspections_Sites_WaterSupplierId_SiteId",
                table: "FogInspections",
                columns: new[] { "WaterSupplierId", "SiteId" },
                principalTable: "Sites",
                principalColumns: new[] { "WaterSupplierId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FogTripTickets_Sites_WaterSupplierId_SiteId",
                table: "FogTripTickets",
                columns: new[] { "WaterSupplierId", "SiteId" },
                principalTable: "Sites",
                principalColumns: new[] { "WaterSupplierId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SiteLogs_Sites_WaterSupplierId_SiteId",
                table: "SiteLogs",
                columns: new[] { "WaterSupplierId", "SiteId" },
                principalTable: "Sites",
                principalColumns: new[] { "WaterSupplierId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
