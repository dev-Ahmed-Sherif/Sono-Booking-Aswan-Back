using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameMarinaOrganizationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarinaOrganizations_Organizations_OrganizationId",
                table: "MarinaOrganizations");

            migrationBuilder.DropForeignKey(
                name: "FK_MarinaOrganizations_TouristMarinas_TouristMarinaId",
                table: "MarinaOrganizations");

            migrationBuilder.DropForeignKey(
                name: "FK_MarinaOrganizations_TouristMarinas_TouristMarinaId1",
                table: "MarinaOrganizations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MarinaOrganizations",
                table: "MarinaOrganizations");

            migrationBuilder.RenameTable(
                name: "MarinaOrganizations",
                newName: "TouristMarinaOrganizations");

            migrationBuilder.RenameIndex(
                name: "IX_MarinaOrganizations_TouristMarinaId1",
                table: "TouristMarinaOrganizations",
                newName: "IX_TouristMarinaOrganizations_TouristMarinaId1");

            migrationBuilder.RenameIndex(
                name: "IX_MarinaOrganizations_TouristMarinaId",
                table: "TouristMarinaOrganizations",
                newName: "IX_TouristMarinaOrganizations_TouristMarinaId");

            migrationBuilder.RenameIndex(
                name: "IX_MarinaOrganizations_OrganizationId",
                table: "TouristMarinaOrganizations",
                newName: "IX_TouristMarinaOrganizations_OrganizationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TouristMarinaOrganizations",
                table: "TouristMarinaOrganizations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TouristMarinaOrganizations_Organizations_OrganizationId",
                table: "TouristMarinaOrganizations",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TouristMarinaOrganizations_TouristMarinas_TouristMarinaId",
                table: "TouristMarinaOrganizations",
                column: "TouristMarinaId",
                principalTable: "TouristMarinas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TouristMarinaOrganizations_TouristMarinas_TouristMarinaId1",
                table: "TouristMarinaOrganizations",
                column: "TouristMarinaId1",
                principalTable: "TouristMarinas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TouristMarinaOrganizations_Organizations_OrganizationId",
                table: "TouristMarinaOrganizations");

            migrationBuilder.DropForeignKey(
                name: "FK_TouristMarinaOrganizations_TouristMarinas_TouristMarinaId",
                table: "TouristMarinaOrganizations");

            migrationBuilder.DropForeignKey(
                name: "FK_TouristMarinaOrganizations_TouristMarinas_TouristMarinaId1",
                table: "TouristMarinaOrganizations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TouristMarinaOrganizations",
                table: "TouristMarinaOrganizations");

            migrationBuilder.RenameTable(
                name: "TouristMarinaOrganizations",
                newName: "MarinaOrganizations");

            migrationBuilder.RenameIndex(
                name: "IX_TouristMarinaOrganizations_TouristMarinaId1",
                table: "MarinaOrganizations",
                newName: "IX_MarinaOrganizations_TouristMarinaId1");

            migrationBuilder.RenameIndex(
                name: "IX_TouristMarinaOrganizations_TouristMarinaId",
                table: "MarinaOrganizations",
                newName: "IX_MarinaOrganizations_TouristMarinaId");

            migrationBuilder.RenameIndex(
                name: "IX_TouristMarinaOrganizations_OrganizationId",
                table: "MarinaOrganizations",
                newName: "IX_MarinaOrganizations_OrganizationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarinaOrganizations",
                table: "MarinaOrganizations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarinaOrganizations_Organizations_OrganizationId",
                table: "MarinaOrganizations",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MarinaOrganizations_TouristMarinas_TouristMarinaId",
                table: "MarinaOrganizations",
                column: "TouristMarinaId",
                principalTable: "TouristMarinas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MarinaOrganizations_TouristMarinas_TouristMarinaId1",
                table: "MarinaOrganizations",
                column: "TouristMarinaId1",
                principalTable: "TouristMarinas",
                principalColumn: "Id");
        }
    }
}
