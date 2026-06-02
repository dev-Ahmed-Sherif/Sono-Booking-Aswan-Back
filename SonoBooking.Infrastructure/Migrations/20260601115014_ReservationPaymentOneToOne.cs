using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReservationPaymentOneToOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IDX_Payments_ReservationId",
                schema: "booking",
                table: "Payments");

            migrationBuilder.CreateIndex(
                name: "UX_Payments_ReservationId",
                schema: "booking",
                table: "Payments",
                column: "ReservationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Payments_ReservationId",
                schema: "booking",
                table: "Payments");

            migrationBuilder.CreateIndex(
                name: "IDX_Payments_ReservationId",
                schema: "booking",
                table: "Payments",
                column: "ReservationId");
        }
    }
}
