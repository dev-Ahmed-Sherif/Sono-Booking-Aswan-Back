using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveApartmentNumberLocationUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Apartment_Number_Location",
                schema: "units",
                table: "Apartments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "UX_Apartment_Number_Location",
                schema: "units",
                table: "Apartments",
                columns: new[] { "ApartmentNumber", "BuildingNumber", "Street", "CityId" },
                unique: true);
        }
    }
}
