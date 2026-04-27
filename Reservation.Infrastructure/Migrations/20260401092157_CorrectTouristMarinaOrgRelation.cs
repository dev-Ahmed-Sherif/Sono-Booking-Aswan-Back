using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CorrectTouristMarinaOrgRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TouristMarinaOrganizations_TouristMarinas_TouristMarinaId1",
                table: "TouristMarinaOrganizations");

            migrationBuilder.DropIndex(
                name: "IX_TouristMarinaOrganizations_TouristMarinaId1",
                table: "TouristMarinaOrganizations");

            migrationBuilder.DropColumn(
                name: "TouristMarinaId1",
                table: "TouristMarinaOrganizations");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "TouristMarinas",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "TouristMarinas",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "TouristMarinas",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "TouristMarinas",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TouristMarinaId1",
                table: "TouristMarinaOrganizations",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TouristMarinaOrganizations_TouristMarinaId1",
                table: "TouristMarinaOrganizations",
                column: "TouristMarinaId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TouristMarinaOrganizations_TouristMarinas_TouristMarinaId1",
                table: "TouristMarinaOrganizations",
                column: "TouristMarinaId1",
                principalTable: "TouristMarinas",
                principalColumn: "Id");
        }
    }
}
