using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class SaveCsiAssembliesWithInspection : Migration
    {
        private const string TableName = "CsiInspectionVisuallyIdentifiedAssemblies";
        private const string InspectionForeignKeyName = "FK_CsiInspectionVisuallyIdentifiedAssemblies_CsiInspections_WaterSupplierId_InspectionId";
        private const string InspectionIndexName = "IX_CsiInspectionVisuallyIdentifiedAssemblies_WaterSupplierId_InspectionId";
        private const string ProfessionalForeignKeyName = "FK_CsiInspectionVisuallyIdentifiedAssemblies_Professionals_ProfessionalId";
        private const string ProfessionalIndexName = "IX_CsiInspectionVisuallyIdentifiedAssemblies_ProfessionalId";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AddedOnInspection",
                table: TableName,
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ProfessionalId",
                table: TableName,
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Rows saved by the earlier SubmissionId-staged form: the test was added on the inspection when
            // it carries the row's SubmissionId and an inspector.
            migrationBuilder.Sql(@"
                UPDATE assemblies
                SET AddedOnInspection = 1
                FROM CsiInspectionVisuallyIdentifiedAssemblies AS assemblies
                INNER JOIN BackflowTests AS tests
                    ON tests.WaterSupplierId = assemblies.WaterSupplierId
                    AND tests.Id = assemblies.TestId
                WHERE tests.InspectorId IS NOT NULL
                    AND ISNULL(assemblies.SubmissionId, '') <> ''
                    AND tests.SubmissionId = assemblies.SubmissionId");

            // Forms that were never submitted: rows without an inspection and tests that never got a site.
            migrationBuilder.Sql("DELETE FROM CsiInspectionVisuallyIdentifiedAssemblies WHERE InspectionId IS NULL");

            migrationBuilder.Sql(@"
                UPDATE BackflowTests
                SET DeletedTime = SYSUTCDATETIME()
                WHERE SiteId IS NULL
                    AND InspectorId IS NOT NULL
                    AND ISNULL(TransactionId, '') = ''
                    AND DeletedTime IS NULL");

            migrationBuilder.Sql(@"
                UPDATE assemblies
                SET ProfessionalId = inspections.ProfessionalId
                FROM CsiInspectionVisuallyIdentifiedAssemblies AS assemblies
                INNER JOIN CsiInspections AS inspections
                    ON inspections.WaterSupplierId = assemblies.WaterSupplierId
                    AND inspections.Id = assemblies.InspectionId");

            // SQL Server cannot alter a column that a foreign key or index uses, so both are rebuilt around it.
            DropInspectionIndexAndForeignKey(migrationBuilder);

            migrationBuilder.AlterColumn<int>(
                name: "InspectionId",
                table: TableName,
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            CreateInspectionIndexAndForeignKey(migrationBuilder);

            migrationBuilder.CreateIndex(
                name: ProfessionalIndexName,
                table: TableName,
                column: "ProfessionalId");

            migrationBuilder.AddForeignKey(
                name: ProfessionalForeignKeyName,
                table: TableName,
                column: "ProfessionalId",
                principalTable: "Professionals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: ProfessionalForeignKeyName, table: TableName);
            migrationBuilder.DropIndex(name: ProfessionalIndexName, table: TableName);

            migrationBuilder.DropColumn(name: "AddedOnInspection", table: TableName);
            migrationBuilder.DropColumn(name: "ProfessionalId", table: TableName);

            DropInspectionIndexAndForeignKey(migrationBuilder);

            migrationBuilder.AlterColumn<int>(
                name: "InspectionId",
                table: TableName,
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            CreateInspectionIndexAndForeignKey(migrationBuilder);
        }

        private static void DropInspectionIndexAndForeignKey(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: InspectionForeignKeyName, table: TableName);
            migrationBuilder.DropIndex(name: InspectionIndexName, table: TableName);
        }

        private static void CreateInspectionIndexAndForeignKey(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: InspectionIndexName,
                table: TableName,
                columns: new[] { "WaterSupplierId", "InspectionId" });

            migrationBuilder.AddForeignKey(
                name: InspectionForeignKeyName,
                table: TableName,
                columns: new[] { "WaterSupplierId", "InspectionId" },
                principalTable: "CsiInspections",
                principalColumns: new[] { "WaterSupplierId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
