using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BackfillReservationCancelationReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE [booking].[Reservations]
                SET [CancelationReason] = NULL
                WHERE [CancelationReason] = N''
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE [booking].[Reservations]
                SET [CancelationReason] = N''
                WHERE [CancelationReason] IS NULL
                """);
        }
    }
}
