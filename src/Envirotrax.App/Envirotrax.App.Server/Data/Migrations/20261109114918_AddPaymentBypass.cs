using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentBypass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBackflowTestingPaymentBypassed",
                table: "ProfessionalUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCsiInspectionPaymentBypassed",
                table: "ProfessionalUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFogInspectionPaymentBypassed",
                table: "ProfessionalUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFogTransportationPaymentBypassed",
                table: "ProfessionalUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaymentBypassed",
                table: "ProfessionalTransactions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBackflowTestingPaymentBypassed",
                table: "ProfessionalUsers");

            migrationBuilder.DropColumn(
                name: "IsCsiInspectionPaymentBypassed",
                table: "ProfessionalUsers");

            migrationBuilder.DropColumn(
                name: "IsFogInspectionPaymentBypassed",
                table: "ProfessionalUsers");

            migrationBuilder.DropColumn(
                name: "IsFogTransportationPaymentBypassed",
                table: "ProfessionalUsers");

            migrationBuilder.DropColumn(
                name: "IsPaymentBypassed",
                table: "ProfessionalTransactions");
        }
    }
}
