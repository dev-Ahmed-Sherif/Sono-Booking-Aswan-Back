using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserJobTitleAndOrganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "AspNetUsers",
                type: "nvarchar(280)",
                maxLength: 280,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Organization",
                table: "AspNetUsers",
                type: "nvarchar(280)",
                maxLength: 280,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Organization",
                table: "AspNetUsers");
        }
    }
}
