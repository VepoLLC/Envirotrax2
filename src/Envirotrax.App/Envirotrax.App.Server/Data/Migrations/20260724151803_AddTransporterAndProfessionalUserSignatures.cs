using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTransporterAndProfessionalUserSignatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SignaturePath",
                table: "ProfessionalUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TransporterSignatureDate",
                table: "FogTripTickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransporterSignaturePath",
                table: "FogTripTickets",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignaturePath",
                table: "ProfessionalUsers");

            migrationBuilder.DropColumn(
                name: "TransporterSignatureDate",
                table: "FogTripTickets");

            migrationBuilder.DropColumn(
                name: "TransporterSignaturePath",
                table: "FogTripTickets");
        }
    }
}
