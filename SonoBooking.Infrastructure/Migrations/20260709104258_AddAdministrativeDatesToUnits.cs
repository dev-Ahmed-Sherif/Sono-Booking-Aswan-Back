using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdministrativeDatesToUnits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "EndAdministrativeDate",
                schema: "units",
                table: "Rooms",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartAdministrativeDate",
                schema: "units",
                table: "Rooms",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndAdministrativeDate",
                schema: "units",
                table: "Beds",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartAdministrativeDate",
                schema: "units",
                table: "Beds",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndAdministrativeDate",
                schema: "units",
                table: "Apartments",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartAdministrativeDate",
                schema: "units",
                table: "Apartments",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndAdministrativeDate",
                schema: "units",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "StartAdministrativeDate",
                schema: "units",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "EndAdministrativeDate",
                schema: "units",
                table: "Beds");

            migrationBuilder.DropColumn(
                name: "StartAdministrativeDate",
                schema: "units",
                table: "Beds");

            migrationBuilder.DropColumn(
                name: "EndAdministrativeDate",
                schema: "units",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "StartAdministrativeDate",
                schema: "units",
                table: "Apartments");
        }
    }
}
