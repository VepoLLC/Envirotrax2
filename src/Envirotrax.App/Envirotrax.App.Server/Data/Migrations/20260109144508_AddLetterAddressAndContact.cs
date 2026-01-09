using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLetterAddressAndContact : Migration
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

            migrationBuilder.AddColumn<int>(
                name: "LetterAddressId",
                table: "WaterSuppliers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LetterContactId",
                table: "WaterSuppliers",
                type: "int",
                nullable: true);

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

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "LetterAddress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LetterAddress", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LetterContact",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FaxNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LetterContact", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WaterSuppliers_LetterAddressId",
                table: "WaterSuppliers",
                column: "LetterAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterSuppliers_LetterContactId",
                table: "WaterSuppliers",
                column: "LetterContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_WaterSuppliers_LetterAddress_LetterAddressId",
                table: "WaterSuppliers",
                column: "LetterAddressId",
                principalTable: "LetterAddress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WaterSuppliers_LetterContact_LetterContactId",
                table: "WaterSuppliers",
                column: "LetterContactId",
                principalTable: "LetterContact",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaterSuppliers_LetterAddress_LetterAddressId",
                table: "WaterSuppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_WaterSuppliers_LetterContact_LetterContactId",
                table: "WaterSuppliers");

            migrationBuilder.DropTable(
                name: "LetterAddress");

            migrationBuilder.DropTable(
                name: "LetterContact");

            migrationBuilder.DropIndex(
                name: "IX_WaterSuppliers_LetterAddressId",
                table: "WaterSuppliers");

            migrationBuilder.DropIndex(
                name: "IX_WaterSuppliers_LetterContactId",
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
                name: "LetterAddressId",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "LetterContactId",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "PwsId",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "State",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "WaterSuppliers");
        }
    }
}
