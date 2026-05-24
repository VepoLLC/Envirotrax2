using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBackflowTestReadingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AirGapValid",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FinalBCClosedTight",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalBCHeldPSID",
                table: "BackflowTests",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FinalCV1ClosedTight",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FinalCV1ClosedTight2",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalCV1HeldPSID",
                table: "BackflowTests",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalCV1HeldPSID2",
                table: "BackflowTests",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FinalCV2ClosedTight",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FinalCV2ClosedTight2",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalCV2HeldPSID",
                table: "BackflowTests",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FinalPvbAirInletFullyOpened",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalPvbAirInletOpenedPSID",
                table: "BackflowTests",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalPvbCVHeldPSID",
                table: "BackflowTests",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalRVOpenedPSID",
                table: "BackflowTests",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalRVOpenedPSID2",
                table: "BackflowTests",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InitBCClosedTight",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "InitBCHeldPSID",
                table: "BackflowTests",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InitBCLeaked",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InitCV1ClosedTight",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InitCV1ClosedTight2",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "InitCV1HeldPSID",
                table: "BackflowTests",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitCV1HeldPSID2",
                table: "BackflowTests",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InitCV1Leaked",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InitCV1Leaked2",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InitCV2ClosedTight",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InitCV2ClosedTight2",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "InitCV2HeldPSID",
                table: "BackflowTests",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitCV2HeldPSID2",
                table: "BackflowTests",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InitCV2Leaked",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InitCV2Leaked2",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InitPvbAirInletDidNotOpen",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InitPvbAirInletFullyOpened",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "InitPvbAirInletOpenedPSID",
                table: "BackflowTests",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitPvbCVHeldPSID",
                table: "BackflowTests",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InitPvbCVLeaked",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InitRVDidNotOpen",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InitRVDidNotOpen2",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "InitRVOpenedPSID",
                table: "BackflowTests",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitRVOpenedPSID2",
                table: "BackflowTests",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Manufacturer2",
                table: "BackflowTests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MeterReadingAfter",
                table: "BackflowTests",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MeterReadingBefore",
                table: "BackflowTests",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MeterRegisters",
                table: "BackflowTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Model2",
                table: "BackflowTests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepairBC",
                table: "BackflowTests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepairBCDetails",
                table: "BackflowTests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepairCV1",
                table: "BackflowTests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepairCV12",
                table: "BackflowTests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepairCV1Details",
                table: "BackflowTests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepairCV1Details2",
                table: "BackflowTests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepairCV2",
                table: "BackflowTests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepairCV22",
                table: "BackflowTests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepairCV2Details",
                table: "BackflowTests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepairCV2Details2",
                table: "BackflowTests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepairRV",
                table: "BackflowTests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepairRV2",
                table: "BackflowTests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepairRVDetails",
                table: "BackflowTests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepairRVDetails2",
                table: "BackflowTests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReplacementAssembly",
                table: "BackflowTests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber2",
                table: "BackflowTests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Size2",
                table: "BackflowTests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AirGapValid",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "FinalBCClosedTight",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "FinalBCHeldPSID",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "FinalCV1ClosedTight",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "FinalCV1ClosedTight2",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "FinalCV1HeldPSID",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "FinalCV1HeldPSID2",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "FinalCV2ClosedTight",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "FinalCV2ClosedTight2",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "FinalCV2HeldPSID",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "FinalPvbAirInletFullyOpened",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "FinalPvbAirInletOpenedPSID",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "FinalPvbCVHeldPSID",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "FinalRVOpenedPSID",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "FinalRVOpenedPSID2",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitBCClosedTight",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitBCHeldPSID",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitBCLeaked",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitCV1ClosedTight",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitCV1ClosedTight2",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitCV1HeldPSID",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitCV1HeldPSID2",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitCV1Leaked",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitCV1Leaked2",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitCV2ClosedTight",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitCV2ClosedTight2",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitCV2HeldPSID",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitCV2HeldPSID2",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitCV2Leaked",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitCV2Leaked2",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitPvbAirInletDidNotOpen",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitPvbAirInletFullyOpened",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitPvbAirInletOpenedPSID",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitPvbCVHeldPSID",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitPvbCVLeaked",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitRVDidNotOpen",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitRVDidNotOpen2",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitRVOpenedPSID",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "InitRVOpenedPSID2",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "Manufacturer2",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "MeterReadingAfter",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "MeterReadingBefore",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "MeterRegisters",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "Model2",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "RepairBC",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "RepairBCDetails",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "RepairCV1",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "RepairCV12",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "RepairCV1Details",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "RepairCV1Details2",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "RepairCV2",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "RepairCV22",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "RepairCV2Details",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "RepairCV2Details2",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "RepairRV",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "RepairRV2",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "RepairRVDetails",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "RepairRVDetails2",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "ReplacementAssembly",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "SerialNumber2",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "Size2",
                table: "BackflowTests");
        }
    }
}
