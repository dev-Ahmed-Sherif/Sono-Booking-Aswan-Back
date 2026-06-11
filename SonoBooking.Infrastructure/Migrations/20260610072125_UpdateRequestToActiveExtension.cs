using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRequestToActiveExtension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequestCatagory",
                schema: "booking",
                table: "Requests",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "ReservationId",
                schema: "booking",
                table: "Requests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IDX_Requests_ReservationId",
                schema: "booking",
                table: "Requests",
                column: "ReservationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Reservation",
                schema: "booking",
                table: "Requests",
                column: "ReservationId",
                principalSchema: "booking",
                principalTable: "Reservations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Reservation",
                schema: "booking",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IDX_Requests_ReservationId",
                schema: "booking",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "RequestCatagory",
                schema: "booking",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "ReservationId",
                schema: "booking",
                table: "Requests");
        }
    }
}
