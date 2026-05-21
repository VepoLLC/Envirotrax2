using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirotrax.App.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorBackflowTests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "BpatState",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "MailingState",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "MasterBpatId",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "PropertyState",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "RejectedBy",
                table: "BackflowTests");

            migrationBuilder.RenameColumn(
                name: "CreationDate",
                table: "BackflowTests",
                newName: "CreatedTime");

            migrationBuilder.AlterColumn<int>(
                name: "BpatId",
                table: "BackflowTests",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedById",
                table: "BackflowTests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BpatStateId",
                table: "BackflowTests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "BackflowTests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedById",
                table: "BackflowTests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedTime",
                table: "BackflowTests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MailingStateId",
                table: "BackflowTests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProfessionalId",
                table: "BackflowTests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PropertyStateId",
                table: "BackflowTests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RejectedById",
                table: "BackflowTests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedById",
                table: "BackflowTests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "BackflowTests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BackflowTests_BpatStateId",
                table: "BackflowTests",
                column: "BpatStateId");

            migrationBuilder.CreateIndex(
                name: "IX_BackflowTests_CreatedById",
                table: "BackflowTests",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BackflowTests_DeletedById",
                table: "BackflowTests",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_BackflowTests_MailingStateId",
                table: "BackflowTests",
                column: "MailingStateId");

            migrationBuilder.CreateIndex(
                name: "IX_BackflowTests_ProfessionalId_BpatId",
                table: "BackflowTests",
                columns: new[] { "ProfessionalId", "BpatId" });

            migrationBuilder.CreateIndex(
                name: "IX_BackflowTests_PropertyStateId",
                table: "BackflowTests",
                column: "PropertyStateId");

            migrationBuilder.CreateIndex(
                name: "IX_BackflowTests_UpdatedById",
                table: "BackflowTests",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BackflowTests_WaterSupplierId_ApprovedById",
                table: "BackflowTests",
                columns: new[] { "WaterSupplierId", "ApprovedById" });

            migrationBuilder.CreateIndex(
                name: "IX_BackflowTests_WaterSupplierId_RejectedById",
                table: "BackflowTests",
                columns: new[] { "WaterSupplierId", "RejectedById" });

            migrationBuilder.AddForeignKey(
                name: "FK_BackflowTests_AspNetUsers_CreatedById",
                table: "BackflowTests",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BackflowTests_AspNetUsers_DeletedById",
                table: "BackflowTests",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BackflowTests_AspNetUsers_UpdatedById",
                table: "BackflowTests",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BackflowTests_ProfessionalUsers_ProfessionalId_BpatId",
                table: "BackflowTests",
                columns: new[] { "ProfessionalId", "BpatId" },
                principalTable: "ProfessionalUsers",
                principalColumns: new[] { "ProfessionalId", "UserId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BackflowTests_States_BpatStateId",
                table: "BackflowTests",
                column: "BpatStateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BackflowTests_States_MailingStateId",
                table: "BackflowTests",
                column: "MailingStateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BackflowTests_States_PropertyStateId",
                table: "BackflowTests",
                column: "PropertyStateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BackflowTests_WaterSupplierUsers_WaterSupplierId_ApprovedById",
                table: "BackflowTests",
                columns: new[] { "WaterSupplierId", "ApprovedById" },
                principalTable: "WaterSupplierUsers",
                principalColumns: new[] { "WaterSupplierId", "UserId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BackflowTests_WaterSupplierUsers_WaterSupplierId_RejectedById",
                table: "BackflowTests",
                columns: new[] { "WaterSupplierId", "RejectedById" },
                principalTable: "WaterSupplierUsers",
                principalColumns: new[] { "WaterSupplierId", "UserId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BackflowTests_AspNetUsers_CreatedById",
                table: "BackflowTests");

            migrationBuilder.DropForeignKey(
                name: "FK_BackflowTests_AspNetUsers_DeletedById",
                table: "BackflowTests");

            migrationBuilder.DropForeignKey(
                name: "FK_BackflowTests_AspNetUsers_UpdatedById",
                table: "BackflowTests");

            migrationBuilder.DropForeignKey(
                name: "FK_BackflowTests_ProfessionalUsers_ProfessionalId_BpatId",
                table: "BackflowTests");

            migrationBuilder.DropForeignKey(
                name: "FK_BackflowTests_States_BpatStateId",
                table: "BackflowTests");

            migrationBuilder.DropForeignKey(
                name: "FK_BackflowTests_States_MailingStateId",
                table: "BackflowTests");

            migrationBuilder.DropForeignKey(
                name: "FK_BackflowTests_States_PropertyStateId",
                table: "BackflowTests");

            migrationBuilder.DropForeignKey(
                name: "FK_BackflowTests_WaterSupplierUsers_WaterSupplierId_ApprovedById",
                table: "BackflowTests");

            migrationBuilder.DropForeignKey(
                name: "FK_BackflowTests_WaterSupplierUsers_WaterSupplierId_RejectedById",
                table: "BackflowTests");

            migrationBuilder.DropIndex(
                name: "IX_BackflowTests_BpatStateId",
                table: "BackflowTests");

            migrationBuilder.DropIndex(
                name: "IX_BackflowTests_CreatedById",
                table: "BackflowTests");

            migrationBuilder.DropIndex(
                name: "IX_BackflowTests_DeletedById",
                table: "BackflowTests");

            migrationBuilder.DropIndex(
                name: "IX_BackflowTests_MailingStateId",
                table: "BackflowTests");

            migrationBuilder.DropIndex(
                name: "IX_BackflowTests_ProfessionalId_BpatId",
                table: "BackflowTests");

            migrationBuilder.DropIndex(
                name: "IX_BackflowTests_PropertyStateId",
                table: "BackflowTests");

            migrationBuilder.DropIndex(
                name: "IX_BackflowTests_UpdatedById",
                table: "BackflowTests");

            migrationBuilder.DropIndex(
                name: "IX_BackflowTests_WaterSupplierId_ApprovedById",
                table: "BackflowTests");

            migrationBuilder.DropIndex(
                name: "IX_BackflowTests_WaterSupplierId_RejectedById",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "BpatStateId",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "DeletedTime",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "MailingStateId",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "ProfessionalId",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "PropertyStateId",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "RejectedById",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "BackflowTests");

            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "BackflowTests");

            migrationBuilder.RenameColumn(
                name: "CreatedTime",
                table: "BackflowTests",
                newName: "CreationDate");

            migrationBuilder.AlterColumn<string>(
                name: "BpatId",
                table: "BackflowTests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "BackflowTests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BpatState",
                table: "BackflowTests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MailingState",
                table: "BackflowTests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MasterBpatId",
                table: "BackflowTests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertyState",
                table: "BackflowTests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedBy",
                table: "BackflowTests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
