using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangePropSiteEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackflowAccountAssignment",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "CsiAccountAssignment",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "FogAccountAssignment",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "UserAccountAssignment",
                table: "Sites");

            migrationBuilder.RenameColumn(
                name: "HasGreaseTrap",
                table: "Sites",
                newName: "GreaseTrapType");

            migrationBuilder.AlterColumn<int>(
                name: "FacilityType",
                table: "Sites",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BackflowAccountAssignmentId",
                table: "Sites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CsiAccountAssignmentId",
                table: "Sites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FogAccountAssignmentId",
                table: "Sites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserAccountAssignmentId",
                table: "Sites",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackflowAccountAssignmentId",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "CsiAccountAssignmentId",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "FogAccountAssignmentId",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "UserAccountAssignmentId",
                table: "Sites");

            migrationBuilder.RenameColumn(
                name: "GreaseTrapType",
                table: "Sites",
                newName: "HasGreaseTrap");

            migrationBuilder.AlterColumn<string>(
                name: "FacilityType",
                table: "Sites",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "BackflowAccountAssignment",
                table: "Sites",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CsiAccountAssignment",
                table: "Sites",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FogAccountAssignment",
                table: "Sites",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAccountAssignment",
                table: "Sites",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
