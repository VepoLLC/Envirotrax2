using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeCsiAssemblyInspectionIdNullable : Migration
    {
        private const string TableName = "CsiInspectionVisuallyIdentifiedAssemblies";
        private const string ForeignKeyName = "FK_CsiInspectionVisuallyIdentifiedAssemblies_CsiInspections_WaterSupplierId_InspectionId";
        private const string IndexName = "IX_CsiInspectionVisuallyIdentifiedAssemblies_WaterSupplierId_InspectionId";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SQL Server cannot alter a column that a foreign key or index uses, so both are rebuilt around it.
            migrationBuilder.DropForeignKey(name: ForeignKeyName, table: TableName);
            migrationBuilder.DropIndex(name: IndexName, table: TableName);

            migrationBuilder.AlterColumn<int>(
                name: "InspectionId",
                table: TableName,
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            CreateIndexAndForeignKey(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rows of never-submitted forms have no inspection and cannot satisfy the restored constraint.
            migrationBuilder.Sql($"DELETE FROM {TableName} WHERE InspectionId IS NULL");

            migrationBuilder.DropForeignKey(name: ForeignKeyName, table: TableName);
            migrationBuilder.DropIndex(name: IndexName, table: TableName);

            migrationBuilder.AlterColumn<int>(
                name: "InspectionId",
                table: TableName,
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            CreateIndexAndForeignKey(migrationBuilder);
        }

        private static void CreateIndexAndForeignKey(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: IndexName,
                table: TableName,
                columns: new[] { "WaterSupplierId", "InspectionId" });

            migrationBuilder.AddForeignKey(
                name: ForeignKeyName,
                table: TableName,
                columns: new[] { "WaterSupplierId", "InspectionId" },
                principalTable: "CsiInspections",
                principalColumns: new[] { "WaterSupplierId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
