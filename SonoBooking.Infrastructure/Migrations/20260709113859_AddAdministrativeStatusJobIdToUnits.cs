using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdministrativeStatusJobIdToUnits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdministrativeStatusJobId",
                schema: "units",
                table: "Rooms",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdministrativeStatusJobId",
                schema: "units",
                table: "Beds",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdministrativeStatusJobId",
                schema: "units",
                table: "Apartments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdministrativeStatusJobId",
                schema: "units",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "AdministrativeStatusJobId",
                schema: "units",
                table: "Beds");

            migrationBuilder.DropColumn(
                name: "AdministrativeStatusJobId",
                schema: "units",
                table: "Apartments");
        }
    }
}
