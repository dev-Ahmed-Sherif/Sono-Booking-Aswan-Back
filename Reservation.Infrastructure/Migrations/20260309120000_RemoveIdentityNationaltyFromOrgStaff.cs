using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIdentityNationaltyFromOrgStaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationStaffs_Nationalities_NationalityId",
                table: "OrganizationStaffs");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationStaffs_NationalityId",
                table: "OrganizationStaffs");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "OrganizationStaffs");

            migrationBuilder.DropColumn(
                name: "IDType",
                table: "OrganizationStaffs");

            migrationBuilder.DropColumn(
                name: "Identity",
                table: "OrganizationStaffs");

            migrationBuilder.DropColumn(
                name: "NationalityId",
                table: "OrganizationStaffs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "OrganizationStaffs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDType",
                table: "OrganizationStaffs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Identity",
                table: "OrganizationStaffs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalityId",
                table: "OrganizationStaffs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStaffs_NationalityId",
                table: "OrganizationStaffs",
                column: "NationalityId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationStaffs_Nationalities_NationalityId",
                table: "OrganizationStaffs",
                column: "NationalityId",
                principalTable: "Nationalities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
