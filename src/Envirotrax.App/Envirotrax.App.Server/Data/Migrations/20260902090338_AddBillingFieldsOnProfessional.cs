using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBillingFieldsOnProfessional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AccountBalance",
                table: "Professionals",
                type: "decimal(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "BillingAddress",
                table: "Professionals",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingCity",
                table: "Professionals",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingFirstName",
                table: "Professionals",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingLastName",
                table: "Professionals",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BillingStateId",
                table: "Professionals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingZipCode",
                table: "Professionals",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Professionals_BillingStateId",
                table: "Professionals",
                column: "BillingStateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Professionals_States_BillingStateId",
                table: "Professionals",
                column: "BillingStateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Professionals_States_BillingStateId",
                table: "Professionals");

            migrationBuilder.DropIndex(
                name: "IX_Professionals_BillingStateId",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "AccountBalance",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "BillingAddress",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "BillingCity",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "BillingFirstName",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "BillingLastName",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "BillingStateId",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "BillingZipCode",
                table: "Professionals");
        }
    }
}
