using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateToBaseAuditInspectionandLicenseandMaintenance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Maintenances",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Maintenances",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Maintenances",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "Maintenances",
                type: "nvarchar(28)",
                maxLength: 28,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Maintenances",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Maintenances",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "Maintenances",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "LicenseApplications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "LicenseApplications",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "LicenseApplications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "LicenseApplications",
                type: "nvarchar(28)",
                maxLength: 28,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "LicenseApplications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "LicenseApplications",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "LicenseApplications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Inspections",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Inspections",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Inspections",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "Inspections",
                type: "nvarchar(28)",
                maxLength: 28,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Inspections",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Inspections",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "Inspections",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "LicenseApplications");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "LicenseApplications");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "LicenseApplications");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "LicenseApplications");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "LicenseApplications");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "LicenseApplications");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "LicenseApplications");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Inspections");
        }
    }
}
