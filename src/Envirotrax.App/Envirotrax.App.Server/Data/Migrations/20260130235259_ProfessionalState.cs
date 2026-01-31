using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProfessionalState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "Professionals",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Professionals_StateId",
                table: "Professionals",
                column: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Professionals_States_StateId",
                table: "Professionals",
                column: "StateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Professionals_States_StateId",
                table: "Professionals");

            migrationBuilder.DropIndex(
                name: "IX_Professionals_StateId",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "Professionals");
        }
    }
}
