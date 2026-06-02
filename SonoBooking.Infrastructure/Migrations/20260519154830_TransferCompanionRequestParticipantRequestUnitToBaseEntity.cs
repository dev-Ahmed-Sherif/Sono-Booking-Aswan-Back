using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TransferCompanionRequestParticipantRequestUnitToBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "booking",
                table: "RequestUnits");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "booking",
                table: "RequestUnits");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                schema: "booking",
                table: "RequestUnits");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                schema: "booking",
                table: "RequestUnits");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                schema: "booking",
                table: "RequestUnits");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "booking",
                table: "RequestUnits");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                schema: "booking",
                table: "RequestUnits");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "booking",
                table: "RequestParticipants");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "booking",
                table: "RequestParticipants");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                schema: "booking",
                table: "RequestParticipants");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                schema: "booking",
                table: "RequestParticipants");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                schema: "booking",
                table: "RequestParticipants");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "booking",
                table: "RequestParticipants");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                schema: "booking",
                table: "RequestParticipants");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Companions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Companions");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Companions");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "Companions");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Companions");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Companions");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Companions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "booking",
                table: "RequestUnits",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "booking",
                table: "RequestUnits",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                schema: "booking",
                table: "RequestUnits",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                schema: "booking",
                table: "RequestUnits",
                type: "nvarchar(28)",
                maxLength: 28,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                schema: "booking",
                table: "RequestUnits",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                schema: "booking",
                table: "RequestUnits",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                schema: "booking",
                table: "RequestUnits",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "booking",
                table: "RequestParticipants",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "booking",
                table: "RequestParticipants",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                schema: "booking",
                table: "RequestParticipants",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                schema: "booking",
                table: "RequestParticipants",
                type: "nvarchar(28)",
                maxLength: 28,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                schema: "booking",
                table: "RequestParticipants",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                schema: "booking",
                table: "RequestParticipants",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                schema: "booking",
                table: "RequestParticipants",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Companions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Companions",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Companions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "Companions",
                type: "nvarchar(28)",
                maxLength: 28,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Companions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Companions",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "Companions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
