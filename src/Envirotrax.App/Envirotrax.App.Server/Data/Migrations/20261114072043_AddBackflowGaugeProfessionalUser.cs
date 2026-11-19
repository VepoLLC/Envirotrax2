using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBackflowGaugeProfessionalUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BackflowGauges_ProfessionalId",
                table: "BackflowGauges");

            migrationBuilder.AddColumn<int>(
                name: "ProfessionalUserId",
                table: "BackflowGauges",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BackflowGauges_ProfessionalId_ProfessionalUserId",
                table: "BackflowGauges",
                columns: new[] { "ProfessionalId", "ProfessionalUserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BackflowGauges_ProfessionalUsers_ProfessionalId_ProfessionalUserId",
                table: "BackflowGauges",
                columns: new[] { "ProfessionalId", "ProfessionalUserId" },
                principalTable: "ProfessionalUsers",
                principalColumns: new[] { "ProfessionalId", "UserId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BackflowGauges_ProfessionalUsers_ProfessionalId_ProfessionalUserId",
                table: "BackflowGauges");

            migrationBuilder.DropIndex(
                name: "IX_BackflowGauges_ProfessionalId_ProfessionalUserId",
                table: "BackflowGauges");

            migrationBuilder.DropColumn(
                name: "ProfessionalUserId",
                table: "BackflowGauges");

            migrationBuilder.CreateIndex(
                name: "IX_BackflowGauges_ProfessionalId",
                table: "BackflowGauges",
                column: "ProfessionalId");
        }
    }
}
