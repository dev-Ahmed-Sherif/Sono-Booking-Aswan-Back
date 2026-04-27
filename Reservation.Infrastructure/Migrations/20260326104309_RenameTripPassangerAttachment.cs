using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameTripPassangerAttachment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Preserve existing data: rename table instead of dropping/recreating.
            migrationBuilder.DropForeignKey(
                name: "FK_PassengerTripAttachments_TripInformations_TripInformationId",
                table: "PassengerTripAttachments");

            migrationBuilder.RenameTable(
                name: "PassengerTripAttachments",
                newName: "TripPassengerAttachments");

            migrationBuilder.RenameIndex(
                name: "IX_PassengerTripAttachments_TripInformationId",
                table: "TripPassengerAttachments",
                newName: "IX_TripPassengerAttachments_TripInformationId");

            migrationBuilder.AddForeignKey(
                name: "FK_TripPassengerAttachments_TripInformations_TripInformationId",
                table: "TripPassengerAttachments",
                column: "TripInformationId",
                principalTable: "TripInformations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripPassengerAttachments_TripInformations_TripInformationId",
                table: "TripPassengerAttachments");

            migrationBuilder.RenameTable(
                name: "TripPassengerAttachments",
                newName: "PassengerTripAttachments");

            migrationBuilder.RenameIndex(
                name: "IX_TripPassengerAttachments_TripInformationId",
                table: "PassengerTripAttachments",
                newName: "IX_PassengerTripAttachments_TripInformationId");

            migrationBuilder.AddForeignKey(
                name: "FK_PassengerTripAttachments_TripInformations_TripInformationId",
                table: "PassengerTripAttachments",
                column: "TripInformationId",
                principalTable: "TripInformations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
