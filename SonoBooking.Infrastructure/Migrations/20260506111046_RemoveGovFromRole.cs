using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGovFromRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoles_Governorate_GovernorateId",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_GovernorateId",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "AspNetRoles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GovernorateId",
                table: "AspNetRoles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_GovernorateId",
                table: "AspNetRoles",
                column: "GovernorateId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoles_Governorate_GovernorateId",
                table: "AspNetRoles",
                column: "GovernorateId",
                principalTable: "Governorate",
                principalColumn: "Id");
        }
    }
}
