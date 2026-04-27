using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameLicenseApplicationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LicenseApplications_Governorates_GovernorateId",
                table: "LicenseApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_LicenseApplications_Organizations_FromOrganizationId",
                table: "LicenseApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_LicenseApplications_Organizations_ToOrganizationId",
                table: "LicenseApplications");

            migrationBuilder.RenameTable(
                name: "LicenseApplications",
                newName: "TouristMarinaLicenseApplications");

            migrationBuilder.RenameIndex(
                name: "IX_LicenseApplications_ToOrganizationId",
                table: "TouristMarinaLicenseApplications",
                newName: "IX_TouristMarinaLicenseApplications_ToOrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_LicenseApplications_GovernorateId",
                table: "TouristMarinaLicenseApplications",
                newName: "IX_TouristMarinaLicenseApplications_GovernorateId");

            migrationBuilder.RenameIndex(
                name: "IX_LicenseApplications_FromOrganizationId",
                table: "TouristMarinaLicenseApplications",
                newName: "IX_TouristMarinaLicenseApplications_FromOrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_TouristMarinaLicenseApplications_Governorates_GovernorateId",
                table: "TouristMarinaLicenseApplications",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TouristMarinaLicenseApplications_Organizations_FromOrganizationId",
                table: "TouristMarinaLicenseApplications",
                column: "FromOrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TouristMarinaLicenseApplications_Organizations_ToOrganizationId",
                table: "TouristMarinaLicenseApplications",
                column: "ToOrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TouristMarinaLicenseApplications_Governorates_GovernorateId",
                table: "TouristMarinaLicenseApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_TouristMarinaLicenseApplications_Organizations_FromOrganizationId",
                table: "TouristMarinaLicenseApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_TouristMarinaLicenseApplications_Organizations_ToOrganizationId",
                table: "TouristMarinaLicenseApplications");

            migrationBuilder.RenameTable(
                name: "TouristMarinaLicenseApplications",
                newName: "LicenseApplications");

            migrationBuilder.RenameIndex(
                name: "IX_TouristMarinaLicenseApplications_ToOrganizationId",
                table: "LicenseApplications",
                newName: "IX_LicenseApplications_ToOrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_TouristMarinaLicenseApplications_GovernorateId",
                table: "LicenseApplications",
                newName: "IX_LicenseApplications_GovernorateId");

            migrationBuilder.RenameIndex(
                name: "IX_TouristMarinaLicenseApplications_FromOrganizationId",
                table: "LicenseApplications",
                newName: "IX_LicenseApplications_FromOrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_LicenseApplications_Governorates_GovernorateId",
                table: "LicenseApplications",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LicenseApplications_Organizations_FromOrganizationId",
                table: "LicenseApplications",
                column: "FromOrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LicenseApplications_Organizations_ToOrganizationId",
                table: "LicenseApplications",
                column: "ToOrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
