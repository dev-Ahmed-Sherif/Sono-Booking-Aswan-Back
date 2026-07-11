using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdministrativeStatusToUnits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AdministrativeStatus",
                schema: "units",
                table: "Rooms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AdministrativeStatus",
                schema: "units",
                table: "Beds",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AdministrativeStatus",
                schema: "units",
                table: "Apartments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdministrativeStatus",
                schema: "units",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "AdministrativeStatus",
                schema: "units",
                table: "Beds");

            migrationBuilder.DropColumn(
                name: "AdministrativeStatus",
                schema: "units",
                table: "Apartments");
        }
    }
}
