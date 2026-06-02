using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRequestUnitAndReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestUnits_Apartment",
                schema: "booking",
                table: "RequestUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Apartment",
                schema: "booking",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Bed",
                schema: "booking",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Room",
                schema: "booking",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_User",
                schema: "booking",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IDX_Reservations_UserId",
                schema: "booking",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_ApartmentId",
                schema: "booking",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_BedId",
                schema: "booking",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_RoomId",
                schema: "booking",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ApartmentId",
                schema: "booking",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "BedId",
                schema: "booking",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "RoomId",
                schema: "booking",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                schema: "booking",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "booking",
                table: "Reservations");

            migrationBuilder.AlterColumn<string>(
                name: "ApartmentId",
                schema: "booking",
                table: "RequestUnits",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestUnits_Apartment",
                schema: "booking",
                table: "RequestUnits",
                column: "ApartmentId",
                principalSchema: "units",
                principalTable: "Apartments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestUnits_Apartment",
                schema: "booking",
                table: "RequestUnits");

            migrationBuilder.AddColumn<string>(
                name: "ApartmentId",
                schema: "booking",
                table: "Reservations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BedId",
                schema: "booking",
                table: "Reservations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomId",
                schema: "booking",
                table: "Reservations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                schema: "booking",
                table: "Reservations",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "booking",
                table: "Reservations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ApartmentId",
                schema: "booking",
                table: "RequestUnits",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IDX_Reservations_UserId",
                schema: "booking",
                table: "Reservations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ApartmentId",
                schema: "booking",
                table: "Reservations",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_BedId",
                schema: "booking",
                table: "Reservations",
                column: "BedId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_RoomId",
                schema: "booking",
                table: "Reservations",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestUnits_Apartment",
                schema: "booking",
                table: "RequestUnits",
                column: "ApartmentId",
                principalSchema: "units",
                principalTable: "Apartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Apartment",
                schema: "booking",
                table: "Reservations",
                column: "ApartmentId",
                principalSchema: "units",
                principalTable: "Apartments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Bed",
                schema: "booking",
                table: "Reservations",
                column: "BedId",
                principalSchema: "units",
                principalTable: "Beds",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Room",
                schema: "booking",
                table: "Reservations",
                column: "RoomId",
                principalSchema: "units",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_User",
                schema: "booking",
                table: "Reservations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
