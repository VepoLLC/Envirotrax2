using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProfessionalTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalTransactions_AspNetUsers_UserId",
                table: "ProfessionalTransactions");

            migrationBuilder.DropIndex(
                name: "IX_ProfessionalTransactions_ProfessionalId",
                table: "ProfessionalTransactions");

            migrationBuilder.DropIndex(
                name: "IX_ProfessionalTransactions_TransactionId",
                table: "ProfessionalTransactions");

            migrationBuilder.DropIndex(
                name: "IX_ProfessionalTransactions_UserId",
                table: "ProfessionalTransactions");

            migrationBuilder.RenameColumn(
                name: "CcCharge",
                table: "ProfessionalTransactions",
                newName: "CardCharge");

            migrationBuilder.RenameColumn(
                name: "CCNumber",
                table: "ProfessionalTransactions",
                newName: "CardNumber");

            migrationBuilder.RenameColumn(
                name: "CCNameOnCard",
                table: "ProfessionalTransactions",
                newName: "NameOnCard");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "ProfessionalTransactions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalTransactions_ProfessionalId_TransactionId",
                table: "ProfessionalTransactions",
                columns: new[] { "ProfessionalId", "TransactionId" },
                unique: true,
                filter: "[TransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalTransactions_ProfessionalId_UserId",
                table: "ProfessionalTransactions",
                columns: new[] { "ProfessionalId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalTransactions_ProfessionalUsers_ProfessionalId_UserId",
                table: "ProfessionalTransactions",
                columns: new[] { "ProfessionalId", "UserId" },
                principalTable: "ProfessionalUsers",
                principalColumns: new[] { "ProfessionalId", "UserId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalTransactions_ProfessionalUsers_ProfessionalId_UserId",
                table: "ProfessionalTransactions");

            migrationBuilder.DropIndex(
                name: "IX_ProfessionalTransactions_ProfessionalId_TransactionId",
                table: "ProfessionalTransactions");

            migrationBuilder.DropIndex(
                name: "IX_ProfessionalTransactions_ProfessionalId_UserId",
                table: "ProfessionalTransactions");

            migrationBuilder.RenameColumn(
                name: "NameOnCard",
                table: "ProfessionalTransactions",
                newName: "CCNameOnCard");

            migrationBuilder.RenameColumn(
                name: "CardNumber",
                table: "ProfessionalTransactions",
                newName: "CCNumber");

            migrationBuilder.RenameColumn(
                name: "CardCharge",
                table: "ProfessionalTransactions",
                newName: "CcCharge");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "ProfessionalTransactions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalTransactions_ProfessionalId",
                table: "ProfessionalTransactions",
                column: "ProfessionalId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalTransactions_TransactionId",
                table: "ProfessionalTransactions",
                column: "TransactionId",
                unique: true,
                filter: "[TransactionId] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalTransactions_UserId",
                table: "ProfessionalTransactions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalTransactions_AspNetUsers_UserId",
                table: "ProfessionalTransactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
