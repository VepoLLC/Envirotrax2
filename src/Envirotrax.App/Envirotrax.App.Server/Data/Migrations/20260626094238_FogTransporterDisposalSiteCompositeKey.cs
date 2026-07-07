using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class FogTransporterDisposalSiteCompositeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FogTransporterDisposalSites",
                schema: "dbo",
                table: "FogTransporterDisposalSites");

            migrationBuilder.DropIndex(
                name: "IX_FogTransporterDisposalSites_ProfessionalId",
                schema: "dbo",
                table: "FogTransporterDisposalSites");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "dbo",
                table: "FogTransporterDisposalSites");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FogTransporterDisposalSites",
                schema: "dbo",
                table: "FogTransporterDisposalSites",
                columns: new[] { "ProfessionalId", "DisposalSiteId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FogTransporterDisposalSites",
                schema: "dbo",
                table: "FogTransporterDisposalSites");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                schema: "dbo",
                table: "FogTransporterDisposalSites",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FogTransporterDisposalSites",
                schema: "dbo",
                table: "FogTransporterDisposalSites",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_FogTransporterDisposalSites_ProfessionalId",
                schema: "dbo",
                table: "FogTransporterDisposalSites",
                column: "ProfessionalId");
        }
    }
}
