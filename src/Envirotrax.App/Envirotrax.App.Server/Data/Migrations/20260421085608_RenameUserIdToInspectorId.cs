using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserIdToInspectorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CsiInspections_ProfessionalUsers_ProfessionalId_UserId",
                table: "CsiInspections");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "CsiInspections",
                newName: "InspectorId");

            migrationBuilder.RenameIndex(
                name: "IX_CsiInspections_ProfessionalId_UserId",
                table: "CsiInspections",
                newName: "IX_CsiInspections_ProfessionalId_InspectorId");

            migrationBuilder.AddForeignKey(
                name: "FK_CsiInspections_ProfessionalUsers_ProfessionalId_InspectorId",
                table: "CsiInspections",
                columns: new[] { "ProfessionalId", "InspectorId" },
                principalTable: "ProfessionalUsers",
                principalColumns: new[] { "ProfessionalId", "UserId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CsiInspections_ProfessionalUsers_ProfessionalId_InspectorId",
                table: "CsiInspections");

            migrationBuilder.RenameColumn(
                name: "InspectorId",
                table: "CsiInspections",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CsiInspections_ProfessionalId_InspectorId",
                table: "CsiInspections",
                newName: "IX_CsiInspections_ProfessionalId_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CsiInspections_ProfessionalUsers_ProfessionalId_UserId",
                table: "CsiInspections",
                columns: new[] { "ProfessionalId", "UserId" },
                principalTable: "ProfessionalUsers",
                principalColumns: new[] { "ProfessionalId", "UserId" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
