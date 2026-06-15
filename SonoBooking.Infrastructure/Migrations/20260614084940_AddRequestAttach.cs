using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestAttach : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Reservation",
                schema: "booking",
                table: "Requests");

            migrationBuilder.RenameColumn(
                name: "ReservationId",
                schema: "booking",
                table: "Requests",
                newName: "PreviousRequestId");

            migrationBuilder.RenameIndex(
                name: "IDX_Requests_ReservationId",
                schema: "booking",
                table: "Requests",
                newName: "IDX_Requests_PreviousRequestId");

            migrationBuilder.CreateTable(
                name: "RequestAttaches",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AttachmentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RequestA__3214EC07A1B2C3D4", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestAttaches_Attachment",
                        column: x => x.AttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RequestAttaches_Request",
                        column: x => x.RequestId,
                        principalSchema: "booking",
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IDX_RequestAttaches_AttachmentId",
                schema: "booking",
                table: "RequestAttaches",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IDX_RequestAttaches_RequestId",
                schema: "booking",
                table: "RequestAttaches",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "UX_RequestAttaches_Request_Attachment",
                schema: "booking",
                table: "RequestAttaches",
                columns: new[] { "RequestId", "AttachmentId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_PreviousRequest",
                schema: "booking",
                table: "Requests",
                column: "PreviousRequestId",
                principalSchema: "booking",
                principalTable: "Requests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_PreviousRequest",
                schema: "booking",
                table: "Requests");

            migrationBuilder.DropTable(
                name: "RequestAttaches",
                schema: "booking");

            migrationBuilder.RenameColumn(
                name: "PreviousRequestId",
                schema: "booking",
                table: "Requests",
                newName: "ReservationId");

            migrationBuilder.RenameIndex(
                name: "IDX_Requests_PreviousRequestId",
                schema: "booking",
                table: "Requests",
                newName: "IDX_Requests_ReservationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Reservation",
                schema: "booking",
                table: "Requests",
                column: "ReservationId",
                principalSchema: "booking",
                principalTable: "Reservations",
                principalColumn: "Id");
        }
    }
}
