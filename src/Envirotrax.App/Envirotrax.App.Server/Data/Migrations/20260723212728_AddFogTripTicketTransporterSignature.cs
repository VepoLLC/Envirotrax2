using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFogTripTicketTransporterSignature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.Sql(@"
IF COL_LENGTH('FogTripTickets', 'TransporterSignatureDate') IS NULL
    ALTER TABLE [FogTripTickets] ADD [TransporterSignatureDate] datetime2 NULL;");

            migrationBuilder.Sql(@"
IF COL_LENGTH('FogTripTickets', 'TransporterSignaturePath') IS NULL
    ALTER TABLE [FogTripTickets] ADD [TransporterSignaturePath] nvarchar(500) NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('FogTripTickets', 'TransporterSignatureDate') IS NOT NULL
    ALTER TABLE [FogTripTickets] DROP COLUMN [TransporterSignatureDate];");

            migrationBuilder.Sql(@"
IF COL_LENGTH('FogTripTickets', 'TransporterSignaturePath') IS NOT NULL
    ALTER TABLE [FogTripTickets] DROP COLUMN [TransporterSignaturePath];");
        }
    }
}
