using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTripChildTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // IMPORTANT: rename only (do not drop/create) to avoid losing existing child rows.
            migrationBuilder.RenameTable(
                name: "MarinaTrips",
                newName: "TripMarinas");

            migrationBuilder.RenameTable(
                name: "NationalityTrips",
                newName: "TripNationalities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "TripMarinas",
                newName: "MarinaTrips");

            migrationBuilder.RenameTable(
                name: "TripNationalities",
                newName: "NationalityTrips");
        }
    }
}
