using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessionalUserForeignKeyToCsiInspection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InspectorId",
                table: "CsiInspections");

            migrationBuilder.DropColumn(
                name: "MasterInspectorId",
                table: "CsiInspections");

            migrationBuilder.AddColumn<int>(
                name: "ProfessionalId",
                table: "CsiInspections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "CsiInspections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CsiInspections_ProfessionalId_UserId",
                table: "CsiInspections",
                columns: new[] { "ProfessionalId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CsiInspections_ProfessionalUsers_ProfessionalId_UserId",
                table: "CsiInspections",
                columns: new[] { "ProfessionalId", "UserId" },
                principalTable: "ProfessionalUsers",
                principalColumns: new[] { "ProfessionalId", "UserId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CsiInspections_Professionals_ProfessionalId",
                table: "CsiInspections",
                column: "ProfessionalId",
                principalTable: "Professionals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CsiInspections_ProfessionalUsers_ProfessionalId_UserId",
                table: "CsiInspections");

            migrationBuilder.DropForeignKey(
                name: "FK_CsiInspections_Professionals_ProfessionalId",
                table: "CsiInspections");

            migrationBuilder.DropIndex(
                name: "IX_CsiInspections_ProfessionalId_UserId",
                table: "CsiInspections");

            migrationBuilder.DropColumn(
                name: "ProfessionalId",
                table: "CsiInspections");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CsiInspections");

            migrationBuilder.AddColumn<string>(
                name: "InspectorId",
                table: "CsiInspections",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MasterInspectorId",
                table: "CsiInspections",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
