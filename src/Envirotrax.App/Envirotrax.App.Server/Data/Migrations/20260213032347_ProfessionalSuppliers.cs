using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProfessionalSuppliers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WaterSupplierProfessionals");

            migrationBuilder.CreateTable(
                name: "ProfessionalWaterSuppliers",
                columns: table => new
                {
                    WaterSupplierId = table.Column<int>(type: "int", nullable: false),
                    ProfessionalId = table.Column<int>(type: "int", nullable: false),
                    HasWiseGuys = table.Column<bool>(type: "bit", nullable: false),
                    HasBackflowTesting = table.Column<bool>(type: "bit", nullable: false),
                    HasCsiInpection = table.Column<bool>(type: "bit", nullable: false),
                    HasFogInspection = table.Column<bool>(type: "bit", nullable: false),
                    HasFogTransportation = table.Column<bool>(type: "bit", nullable: false),
                    IsBanned = table.Column<bool>(type: "bit", nullable: false),
                    ResidentialFee = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    CommercialFee = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    FogFee = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionalWaterSuppliers", x => x.WaterSupplierId);
                    table.ForeignKey(
                        name: "FK_ProfessionalWaterSuppliers_Professionals_ProfessionalId",
                        column: x => x.ProfessionalId,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfessionalWaterSuppliers_WaterSuppliers_WaterSupplierId",
                        column: x => x.WaterSupplierId,
                        principalTable: "WaterSuppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalWaterSuppliers_ProfessionalId",
                table: "ProfessionalWaterSuppliers",
                column: "ProfessionalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfessionalWaterSuppliers");

            migrationBuilder.CreateTable(
                name: "WaterSupplierProfessionals",
                columns: table => new
                {
                    WaterSupplierId = table.Column<int>(type: "int", nullable: false),
                    ProfessionalId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterSupplierProfessionals", x => new { x.WaterSupplierId, x.ProfessionalId });
                    table.ForeignKey(
                        name: "FK_WaterSupplierProfessionals_Professionals_ProfessionalId",
                        column: x => x.ProfessionalId,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WaterSupplierProfessionals_WaterSuppliers_WaterSupplierId",
                        column: x => x.WaterSupplierId,
                        principalTable: "WaterSuppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WaterSupplierProfessionals_ProfessionalId",
                table: "WaterSupplierProfessionals",
                column: "ProfessionalId");
        }
    }
}
