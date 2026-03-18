using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCsiSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CsiSettings",
                columns: table => new
                {
                    WaterSupplierId = table.Column<int>(type: "int", nullable: false),
                    ModificationGracePeriodDays = table.Column<int>(type: "int", nullable: true),
                    NewlyCreatedBackflowTestExpirationDays = table.Column<int>(type: "int", nullable: false),
                    RequireInspectionImages = table.Column<bool>(type: "bit", nullable: false),
                    ImpendingNotice1 = table.Column<int>(type: "int", nullable: false),
                    ImpendingNotice2 = table.Column<int>(type: "int", nullable: false),
                    PastDueNotice1 = table.Column<int>(type: "int", nullable: false),
                    PastDueNotice2 = table.Column<int>(type: "int", nullable: false),
                    NonCompliant1 = table.Column<int>(type: "int", nullable: false),
                    NonCompliant2 = table.Column<int>(type: "int", nullable: false),
                    ImpendingLettersBackgroundColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    ImpendingLettersForegroundColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    ImpendingLettersBorderColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    PastDueLettersBackgroundColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    PastDueLettersForegroundColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    PastDueLettersBorderColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    NonCompliantLettersBackgroundColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    NonCompliantLettersForegroundColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    NonCompliantLettersBorderColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CsiSettings", x => x.WaterSupplierId);
                    table.ForeignKey(
                        name: "FK_CsiSettings_WaterSuppliers_WaterSupplierId",
                        column: x => x.WaterSupplierId,
                        principalTable: "WaterSuppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CsiSettings");
        }
    }
}
