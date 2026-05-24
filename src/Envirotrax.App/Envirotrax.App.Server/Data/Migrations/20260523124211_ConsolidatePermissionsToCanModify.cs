using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConsolidatePermissionsToCanModify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CanEdit",
                table: "RolePermissions",
                newName: "CanModify");

            migrationBuilder.RenameColumn(
                name: "CanEdit",
                table: "Permissions",
                newName: "CanModify");

            migrationBuilder.Sql("UPDATE RolePermissions SET CanModify = 1 WHERE CanCreate = 1 OR CanModify = 1");
            migrationBuilder.Sql("UPDATE Permissions SET CanModify = 1 WHERE CanCreate = 1 OR CanModify = 1");

            migrationBuilder.DropColumn(
                name: "CanCreate",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "CanCreate",
                table: "Permissions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CanModify",
                table: "RolePermissions",
                newName: "CanEdit");

            migrationBuilder.RenameColumn(
                name: "CanModify",
                table: "Permissions",
                newName: "CanEdit");

            migrationBuilder.AddColumn<bool>(
                name: "CanCreate",
                table: "RolePermissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanCreate",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
