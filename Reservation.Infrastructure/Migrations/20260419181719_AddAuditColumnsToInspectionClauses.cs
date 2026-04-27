using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditColumnsToInspectionClauses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "InspectionClauses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "InspectionClauses",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "InspectionClauses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "InspectionClauses",
                type: "nvarchar(28)",
                maxLength: 28,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "InspectionClauses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "InspectionClauses",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "InspectionClauses",
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
                table: "InspectionClauses");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "InspectionClauses");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "InspectionClauses");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "InspectionClauses");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "InspectionClauses");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "InspectionClauses");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "InspectionClauses");
        }
    }
}
