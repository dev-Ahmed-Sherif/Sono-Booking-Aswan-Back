using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationCategoryToOrganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppliedOn",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "TouristMarinaNumber",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Governorates");

            migrationBuilder.RenameColumn(
                name: "OrganizationTypeId",
                table: "Organizations",
                newName: "OrganizationType");

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                table: "Organizations",
                type: "nvarchar(280)",
                maxLength: 280,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                table: "Organizations",
                type: "nvarchar(280)",
                maxLength: 280,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Organizations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Organizations",
                type: "nvarchar(35)",
                maxLength: 35,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Organizations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Organizations",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Organizations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "Organizations",
                type: "nvarchar(28)",
                maxLength: 28,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Organizations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Organizations",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "Organizations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrganizationCategoryId",
                table: "Organizations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Governorates",
                type: "nvarchar(35)",
                maxLength: 35,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "Governorates",
                type: "nvarchar(280)",
                maxLength: 280,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Governorates",
                type: "nvarchar(280)",
                maxLength: 280,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrganizationCategories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_OrganizationCategoryId",
                table: "Organizations",
                column: "OrganizationCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_OrganizationCategories_OrganizationCategoryId",
                table: "Organizations",
                column: "OrganizationCategoryId",
                principalTable: "OrganizationCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_OrganizationCategories_OrganizationCategoryId",
                table: "Organizations");

            migrationBuilder.DropTable(
                name: "OrganizationCategories");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_OrganizationCategoryId",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "OrganizationCategoryId",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "Governorates");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Governorates");

            migrationBuilder.RenameColumn(
                name: "OrganizationType",
                table: "Organizations",
                newName: "OrganizationTypeId");

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                table: "Organizations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(280)",
                oldMaxLength: 280,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                table: "Organizations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(280)",
                oldMaxLength: 280);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Organizations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Organizations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(35)",
                oldMaxLength: 35);

            migrationBuilder.AddColumn<int>(
                name: "AppliedOn",
                table: "Organizations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TouristMarinaNumber",
                table: "Organizations",
                type: "int",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Governorates",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(35)",
                oldMaxLength: 35);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Governorates",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
