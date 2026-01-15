using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWaterSupplierEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactName",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FaxNumber",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LetterAddress",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LetterCity",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LetterCompanyName",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LetterContactAddress",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LetterContactCity",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LetterContactCompanyName",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LetterContactContactName",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LetterContactEmailAddress",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LetterContactFaxNumber",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LetterContactName",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LetterContactPhoneNumber",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LetterContactStateId",
                table: "WaterSuppliers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LetterContactZipCode",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LetterStateId",
                table: "WaterSuppliers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LetterZipCode",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PwsId",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "WaterSuppliers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WaterSuppliers_LetterContactStateId",
                table: "WaterSuppliers",
                column: "LetterContactStateId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterSuppliers_LetterStateId",
                table: "WaterSuppliers",
                column: "LetterStateId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterSuppliers_StateId",
                table: "WaterSuppliers",
                column: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_WaterSuppliers_States_LetterContactStateId",
                table: "WaterSuppliers",
                column: "LetterContactStateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WaterSuppliers_States_LetterStateId",
                table: "WaterSuppliers",
                column: "LetterStateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WaterSuppliers_States_StateId",
                table: "WaterSuppliers",
                column: "StateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaterSuppliers_States_LetterContactStateId",
                table: "WaterSuppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_WaterSuppliers_States_LetterStateId",
                table: "WaterSuppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_WaterSuppliers_States_StateId",
                table: "WaterSuppliers");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropIndex(
                name: "IX_WaterSuppliers_LetterContactStateId",
                table: "WaterSuppliers");

            migrationBuilder.DropIndex(
                name: "IX_WaterSuppliers_LetterStateId",
                table: "WaterSuppliers");

            migrationBuilder.DropIndex(
                name: "IX_WaterSuppliers_StateId",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "City",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "ContactName",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "FaxNumber",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "LetterAddress",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "LetterCity",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "LetterCompanyName",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "LetterContactAddress",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "LetterContactCity",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "LetterContactCompanyName",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "LetterContactContactName",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "LetterContactEmailAddress",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "LetterContactFaxNumber",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "LetterContactName",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "LetterContactPhoneNumber",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "LetterContactStateId",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "LetterContactZipCode",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "LetterStateId",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "LetterZipCode",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "PwsId",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "WaterSuppliers");
        }
    }
}
