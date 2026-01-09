using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStateEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "State",
                table: "LetterContact");

            migrationBuilder.DropColumn(
                name: "State",
                table: "LetterAddress");

            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "WaterSuppliers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "LetterContact",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "LetterAddress",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WaterSuppliers_StateId",
                table: "WaterSuppliers",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_LetterContact_StateId",
                table: "LetterContact",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_LetterAddress_StateId",
                table: "LetterAddress",
                column: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_LetterAddress_States_StateId",
                table: "LetterAddress",
                column: "StateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LetterContact_States_StateId",
                table: "LetterContact",
                column: "StateId",
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
                name: "FK_LetterAddress_States_StateId",
                table: "LetterAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_LetterContact_States_StateId",
                table: "LetterContact");

            migrationBuilder.DropForeignKey(
                name: "FK_WaterSuppliers_States_StateId",
                table: "WaterSuppliers");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropIndex(
                name: "IX_WaterSuppliers_StateId",
                table: "WaterSuppliers");

            migrationBuilder.DropIndex(
                name: "IX_LetterContact_StateId",
                table: "LetterContact");

            migrationBuilder.DropIndex(
                name: "IX_LetterAddress_StateId",
                table: "LetterAddress");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "LetterContact");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "LetterAddress");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "WaterSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "LetterContact",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "LetterAddress",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
