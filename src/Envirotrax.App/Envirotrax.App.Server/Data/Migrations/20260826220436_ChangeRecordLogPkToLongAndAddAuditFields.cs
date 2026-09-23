using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRecordLogPkToLongAndAddAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecordLogs_AspNetUsers_UserId",
                table: "RecordLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RecordLogs",
                table: "RecordLogs");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "RecordLogs",
                newName: "CreatedById");

            migrationBuilder.RenameColumn(
                name: "LogDate",
                table: "RecordLogs",
                newName: "CreatedTime");

            migrationBuilder.RenameIndex(
                name: "IX_RecordLogs_UserId",
                table: "RecordLogs",
                newName: "IX_RecordLogs_CreatedById");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "RecordLogs",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "WaterSupplierId",
                table: "RecordLogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ProfessionalId",
                table: "RecordLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecordLogs",
                table: "RecordLogs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RecordLogs_ProfessionalId",
                table: "RecordLogs",
                column: "ProfessionalId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordLogs_TableName_RecordId_WaterSupplierId_ProfessionalId",
                table: "RecordLogs",
                columns: new[] { "TableName", "RecordId", "WaterSupplierId", "ProfessionalId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecordLogs_WaterSupplierId",
                table: "RecordLogs",
                column: "WaterSupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecordLogs_AspNetUsers_CreatedById",
                table: "RecordLogs",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RecordLogs_Professionals_ProfessionalId",
                table: "RecordLogs",
                column: "ProfessionalId",
                principalTable: "Professionals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecordLogs_AspNetUsers_CreatedById",
                table: "RecordLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_RecordLogs_Professionals_ProfessionalId",
                table: "RecordLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RecordLogs",
                table: "RecordLogs");

            migrationBuilder.DropIndex(
                name: "IX_RecordLogs_ProfessionalId",
                table: "RecordLogs");

            migrationBuilder.DropIndex(
                name: "IX_RecordLogs_TableName_RecordId_WaterSupplierId_ProfessionalId",
                table: "RecordLogs");

            migrationBuilder.DropIndex(
                name: "IX_RecordLogs_WaterSupplierId",
                table: "RecordLogs");

            migrationBuilder.DropColumn(
                name: "ProfessionalId",
                table: "RecordLogs");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "RecordLogs",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "CreatedTime",
                table: "RecordLogs",
                newName: "LogDate");

            migrationBuilder.RenameIndex(
                name: "IX_RecordLogs_CreatedById",
                table: "RecordLogs",
                newName: "IX_RecordLogs_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "WaterSupplierId",
                table: "RecordLogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "RecordLogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecordLogs",
                table: "RecordLogs",
                columns: new[] { "WaterSupplierId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_RecordLogs_AspNetUsers_UserId",
                table: "RecordLogs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
