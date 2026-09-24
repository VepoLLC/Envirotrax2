using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProfessionalWaterSupplierPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfessionalWaterSuppliers",
                table: "ProfessionalWaterSuppliers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfessionalWaterSuppliers",
                table: "ProfessionalWaterSuppliers",
                columns: new[] { "WaterSupplierId", "ProfessionalId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfessionalWaterSuppliers",
                table: "ProfessionalWaterSuppliers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfessionalWaterSuppliers",
                table: "ProfessionalWaterSuppliers",
                column: "WaterSupplierId");
        }
    }
}
