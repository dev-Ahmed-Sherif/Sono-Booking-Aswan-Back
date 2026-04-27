using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGovIdFromChildTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FloatingUnitOrganizations_Governorates_GovernorateId",
                table: "FloatingUnitOrganizations");

            migrationBuilder.DropForeignKey(
                name: "FK_MarinaOrganizations_Governorates_GovernorateId",
                table: "MarinaOrganizations");

            migrationBuilder.DropForeignKey(
                name: "FK_MarinaTrips_Governorates_GovernorateId",
                table: "MarinaTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationStaffs_Governorates_GovernorateId",
                table: "OrganizationStaffs");

            migrationBuilder.DropForeignKey(
                name: "FK_TripGeos_Governorates_GovernorateId",
                table: "TripGeos");

            migrationBuilder.DropForeignKey(
                name: "FK_TripInformations_Governorates_GovernorateId",
                table: "TripInformations");

            migrationBuilder.DropIndex(
                name: "IX_TripGeos_GovernorateId",
                table: "TripGeos");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationStaffs_GovernorateId",
                table: "OrganizationStaffs");

            migrationBuilder.DropIndex(
                name: "IX_MarinaTrips_GovernorateId",
                table: "MarinaTrips");

            migrationBuilder.DropIndex(
                name: "IX_MarinaOrganizations_GovernorateId",
                table: "MarinaOrganizations");

            migrationBuilder.DropIndex(
                name: "IX_FloatingUnitOrganizations_GovernorateId",
                table: "FloatingUnitOrganizations");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "TripGeos");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "OrganizationStaffs");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "MarinaTrips");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "MarinaOrganizations");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "FloatingUnitOrganizations");

            migrationBuilder.AlterColumn<string>(
                name: "RouteId",
                table: "TripInformations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FloatingUnitId",
                table: "TripInformations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TripInformations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "TripInformations",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "TripInformations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "TripInformations",
                type: "nvarchar(28)",
                maxLength: 28,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "TripInformations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "TripInformations",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "TripInformations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TouristMarinas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "TouristMarinas",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "TouristMarinas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "TouristMarinas",
                type: "nvarchar(28)",
                maxLength: 28,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "TouristMarinas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "TouristMarinas",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "TouristMarinas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_TripInformations_Governorates_GovernorateId",
                table: "TripInformations",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripInformations_Governorates_GovernorateId",
                table: "TripInformations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TripInformations");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TripInformations");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "TripInformations");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "TripInformations");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "TripInformations");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "TripInformations");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "TripInformations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TouristMarinas");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TouristMarinas");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "TouristMarinas");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "TouristMarinas");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "TouristMarinas");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "TouristMarinas");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "TouristMarinas");

            migrationBuilder.AlterColumn<string>(
                name: "RouteId",
                table: "TripInformations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FloatingUnitId",
                table: "TripInformations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "GovernorateId",
                table: "TripGeos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernorateId",
                table: "OrganizationStaffs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernorateId",
                table: "MarinaTrips",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernorateId",
                table: "MarinaOrganizations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernorateId",
                table: "FloatingUnitOrganizations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TripGeos_GovernorateId",
                table: "TripGeos",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStaffs_GovernorateId",
                table: "OrganizationStaffs",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_MarinaTrips_GovernorateId",
                table: "MarinaTrips",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_MarinaOrganizations_GovernorateId",
                table: "MarinaOrganizations",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_FloatingUnitOrganizations_GovernorateId",
                table: "FloatingUnitOrganizations",
                column: "GovernorateId");

            migrationBuilder.AddForeignKey(
                name: "FK_FloatingUnitOrganizations_Governorates_GovernorateId",
                table: "FloatingUnitOrganizations",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MarinaOrganizations_Governorates_GovernorateId",
                table: "MarinaOrganizations",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MarinaTrips_Governorates_GovernorateId",
                table: "MarinaTrips",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationStaffs_Governorates_GovernorateId",
                table: "OrganizationStaffs",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TripGeos_Governorates_GovernorateId",
                table: "TripGeos",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TripInformations_Governorates_GovernorateId",
                table: "TripInformations",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
