using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCsiInspectionResultComputedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "InspectionResult",
                table: "CsiInspections",
                type: "bit",
                nullable: false,
                computedColumnSql: "CASE WHEN [Compliance1] = 1 AND [Compliance2] = 1 AND [Compliance3] = 1 AND [Compliance4] = 1 AND [Compliance5] = 1 AND [Compliance6] = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END",
                stored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InspectionResult",
                table: "CsiInspections");
        }
    }
}
