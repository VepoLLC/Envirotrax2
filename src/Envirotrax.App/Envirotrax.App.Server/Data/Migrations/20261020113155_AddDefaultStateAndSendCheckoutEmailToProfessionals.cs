using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultStateAndSendCheckoutEmailToProfessionals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultStateId",
                table: "Professionals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SendCheckoutEmail",
                table: "Professionals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Professionals_DefaultStateId",
                table: "Professionals",
                column: "DefaultStateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Professionals_States_DefaultStateId",
                table: "Professionals",
                column: "DefaultStateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Professionals_States_DefaultStateId",
                table: "Professionals");

            migrationBuilder.DropIndex(
                name: "IX_Professionals_DefaultStateId",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "DefaultStateId",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "SendCheckoutEmail",
                table: "Professionals");
        }
    }
}
