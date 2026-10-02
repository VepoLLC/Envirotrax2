using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessionalTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BalanceLockedUntil",
                table: "Professionals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProfessionalTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProfessionalId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    ReferenceDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BalanceAdjustment = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    CcCharge = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    AmountShare = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    CCNameOnCard = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CCNumber = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionalTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfessionalTransactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfessionalTransactions_Professionals_ProfessionalId",
                        column: x => x.ProfessionalId,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Professionals_AccountBalance_NonNegative",
                table: "Professionals",
                sql: "[AccountBalance] >= 0");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfessionalTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Professionals_AccountBalance_NonNegative",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "BalanceLockedUntil",
                table: "Professionals");
        }
    }
}
