using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBackflowSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BackflowSettings",
                columns: table => new
                {
                    WaterSupplierId = table.Column<int>(type: "int", nullable: false),
                    TestingMethod = table.Column<int>(type: "int", nullable: false),
                    GracePeriodDays = table.Column<int>(type: "int", nullable: true),
                    AdjustBackflowCreepingDates = table.Column<bool>(type: "bit", nullable: false),
                    NewInstallationsRequireApproval = table.Column<bool>(type: "bit", nullable: false),
                    ReplacementsRequireApproval = table.Column<bool>(type: "bit", nullable: false),
                    DetectorAssembliesRequireMeterReading = table.Column<bool>(type: "bit", nullable: false),
                    OutOfServiceRequiresApproval = table.Column<bool>(type: "bit", nullable: false),
                    OutOfServiceType = table.Column<int>(type: "int", nullable: false),
                    RequireBackflowTestImages = table.Column<bool>(type: "bit", nullable: false),
                    ExpiringNotice1 = table.Column<int>(type: "int", nullable: false),
                    ExpiringNotice2 = table.Column<int>(type: "int", nullable: false),
                    ExpiredNotice1 = table.Column<int>(type: "int", nullable: false),
                    ExpiredNotice2 = table.Column<int>(type: "int", nullable: false),
                    BackflowNonCompliant1 = table.Column<int>(type: "int", nullable: false),
                    BackflowNonCompliant2 = table.Column<int>(type: "int", nullable: false),
                    ShowWaterMeterNumber = table.Column<bool>(type: "bit", nullable: false),
                    ShowRainSensor = table.Column<bool>(type: "bit", nullable: false),
                    ShowOSSF = table.Column<bool>(type: "bit", nullable: false),
                    ShowPermitNumber = table.Column<bool>(type: "bit", nullable: false),
                    ExpiringLettersBackgroundColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    ExpiringLettersForegroundColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    ExpiringLettersBorderColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    ExpiredLettersBackgroundColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    ExpiredLettersForegroundColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    ExpiredLettersBorderColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    NonCompliantLettersBackgroundColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    NonCompliantLettersForegroundColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    NonCompliantLettersBorderColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackflowSettings", x => x.WaterSupplierId);
                    table.ForeignKey(
                        name: "FK_BackflowSettings_WaterSuppliers_WaterSupplierId",
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
                name: "BackflowSettings");
        }
    }
}
