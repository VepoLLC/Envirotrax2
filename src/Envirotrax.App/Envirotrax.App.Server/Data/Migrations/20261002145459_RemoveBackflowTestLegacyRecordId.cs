using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBackflowTestLegacyRecordId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BackflowTests_LegacyRecordId",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "LegacyRecordId",
                table: "BackflowTests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LegacyRecordId",
                table: "BackflowTests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BackflowTests_LegacyRecordId",
                table: "BackflowTests",
                column: "LegacyRecordId");
        }
    }
}
