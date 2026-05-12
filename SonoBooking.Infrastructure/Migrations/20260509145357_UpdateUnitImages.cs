using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUnitImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "units",
                table: "UnitImages");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "units",
                table: "UnitImages");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                schema: "units",
                table: "UnitImages");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                schema: "units",
                table: "UnitImages");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                schema: "units",
                table: "UnitImages");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "units",
                table: "UnitImages");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                schema: "units",
                table: "UnitImages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "units",
                table: "UnitImages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "units",
                table: "UnitImages",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                schema: "units",
                table: "UnitImages",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                schema: "units",
                table: "UnitImages",
                type: "nvarchar(28)",
                maxLength: 28,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                schema: "units",
                table: "UnitImages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                schema: "units",
                table: "UnitImages",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                schema: "units",
                table: "UnitImages",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
