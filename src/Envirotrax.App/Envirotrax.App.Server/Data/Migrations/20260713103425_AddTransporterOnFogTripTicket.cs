using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTransporterOnFogTripTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FogTripTickets_ProfessionalId",
                table: "FogTripTickets");

            migrationBuilder.AddColumn<int>(
                name: "TransporterId",
                table: "FogTripTickets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FogTripTickets_ProfessionalId_TransporterId",
                table: "FogTripTickets",
                columns: new[] { "ProfessionalId", "TransporterId" });

            migrationBuilder.AddForeignKey(
                name: "FK_FogTripTickets_ProfessionalUsers_ProfessionalId_TransporterId",
                table: "FogTripTickets",
                columns: new[] { "ProfessionalId", "TransporterId" },
                principalTable: "ProfessionalUsers",
                principalColumns: new[] { "ProfessionalId", "UserId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FogTripTickets_ProfessionalUsers_ProfessionalId_TransporterId",
                table: "FogTripTickets");

            migrationBuilder.DropIndex(
                name: "IX_FogTripTickets_ProfessionalId_TransporterId",
                table: "FogTripTickets");

            migrationBuilder.DropColumn(
                name: "TransporterId",
                table: "FogTripTickets");

            migrationBuilder.CreateIndex(
                name: "IX_FogTripTickets_ProfessionalId",
                table: "FogTripTickets",
                column: "ProfessionalId");
        }
    }
}
