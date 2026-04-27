using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGovIdRelationForTrackerAndNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GovernorateId",
                table: "TripInformations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernorateId",
                table: "TripGeos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernorateId",
                table: "TouristMarinas",
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
                table: "Notifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernorateId",
                table: "NotificationGroups",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernorateId",
                table: "NationalityTrips",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernorateId",
                table: "MessagingGroups",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernorateId",
                table: "Messages",
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
                table: "Maintenances",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernorateId",
                table: "LicenseApplications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernorateId",
                table: "Inspections",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernorateId",
                table: "FloatingUnitStaffs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernorateId",
                table: "FloatingUnits",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernorateId",
                table: "FloatingUnitOrganizations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernorateId",
                table: "Accidents",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TripInformations_GovernorateId",
                table: "TripInformations",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_TripGeos_GovernorateId",
                table: "TripGeos",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_TouristMarinas_GovernorateId",
                table: "TouristMarinas",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStaffs_GovernorateId",
                table: "OrganizationStaffs",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_GovernorateId",
                table: "Notifications",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationGroups_GovernorateId",
                table: "NotificationGroups",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_NationalityTrips_GovernorateId",
                table: "NationalityTrips",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_MessagingGroups_GovernorateId",
                table: "MessagingGroups",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_GovernorateId",
                table: "Messages",
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
                name: "IX_Maintenances_GovernorateId",
                table: "Maintenances",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseApplications_GovernorateId",
                table: "LicenseApplications",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_GovernorateId",
                table: "Inspections",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_FloatingUnitStaffs_GovernorateId",
                table: "FloatingUnitStaffs",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_FloatingUnits_GovernorateId",
                table: "FloatingUnits",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_FloatingUnitOrganizations_GovernorateId",
                table: "FloatingUnitOrganizations",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_Accidents_GovernorateId",
                table: "Accidents",
                column: "GovernorateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accidents_Governorates_GovernorateId",
                table: "Accidents",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FloatingUnitOrganizations_Governorates_GovernorateId",
                table: "FloatingUnitOrganizations",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FloatingUnits_Governorates_GovernorateId",
                table: "FloatingUnits",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FloatingUnitStaffs_Governorates_GovernorateId",
                table: "FloatingUnitStaffs",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Inspections_Governorates_GovernorateId",
                table: "Inspections",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LicenseApplications_Governorates_GovernorateId",
                table: "LicenseApplications",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_Governorates_GovernorateId",
                table: "Maintenances",
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
                name: "FK_Messages_Governorates_GovernorateId",
                table: "Messages",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MessagingGroups_Governorates_GovernorateId",
                table: "MessagingGroups",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NationalityTrips_Governorates_GovernorateId",
                table: "NationalityTrips",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationGroups_Governorates_GovernorateId",
                table: "NotificationGroups",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Governorates_GovernorateId",
                table: "Notifications",
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
                name: "FK_TouristMarinas_Governorates_GovernorateId",
                table: "TouristMarinas",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accidents_Governorates_GovernorateId",
                table: "Accidents");

            migrationBuilder.DropForeignKey(
                name: "FK_FloatingUnitOrganizations_Governorates_GovernorateId",
                table: "FloatingUnitOrganizations");

            migrationBuilder.DropForeignKey(
                name: "FK_FloatingUnits_Governorates_GovernorateId",
                table: "FloatingUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_FloatingUnitStaffs_Governorates_GovernorateId",
                table: "FloatingUnitStaffs");

            migrationBuilder.DropForeignKey(
                name: "FK_Inspections_Governorates_GovernorateId",
                table: "Inspections");

            migrationBuilder.DropForeignKey(
                name: "FK_LicenseApplications_Governorates_GovernorateId",
                table: "LicenseApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Governorates_GovernorateId",
                table: "Maintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_MarinaOrganizations_Governorates_GovernorateId",
                table: "MarinaOrganizations");

            migrationBuilder.DropForeignKey(
                name: "FK_MarinaTrips_Governorates_GovernorateId",
                table: "MarinaTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Governorates_GovernorateId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_MessagingGroups_Governorates_GovernorateId",
                table: "MessagingGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_NationalityTrips_Governorates_GovernorateId",
                table: "NationalityTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationGroups_Governorates_GovernorateId",
                table: "NotificationGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Governorates_GovernorateId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationStaffs_Governorates_GovernorateId",
                table: "OrganizationStaffs");

            migrationBuilder.DropForeignKey(
                name: "FK_TouristMarinas_Governorates_GovernorateId",
                table: "TouristMarinas");

            migrationBuilder.DropForeignKey(
                name: "FK_TripGeos_Governorates_GovernorateId",
                table: "TripGeos");

            migrationBuilder.DropForeignKey(
                name: "FK_TripInformations_Governorates_GovernorateId",
                table: "TripInformations");

            migrationBuilder.DropIndex(
                name: "IX_TripInformations_GovernorateId",
                table: "TripInformations");

            migrationBuilder.DropIndex(
                name: "IX_TripGeos_GovernorateId",
                table: "TripGeos");

            migrationBuilder.DropIndex(
                name: "IX_TouristMarinas_GovernorateId",
                table: "TouristMarinas");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationStaffs_GovernorateId",
                table: "OrganizationStaffs");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_GovernorateId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_NotificationGroups_GovernorateId",
                table: "NotificationGroups");

            migrationBuilder.DropIndex(
                name: "IX_NationalityTrips_GovernorateId",
                table: "NationalityTrips");

            migrationBuilder.DropIndex(
                name: "IX_MessagingGroups_GovernorateId",
                table: "MessagingGroups");

            migrationBuilder.DropIndex(
                name: "IX_Messages_GovernorateId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_MarinaTrips_GovernorateId",
                table: "MarinaTrips");

            migrationBuilder.DropIndex(
                name: "IX_MarinaOrganizations_GovernorateId",
                table: "MarinaOrganizations");

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_GovernorateId",
                table: "Maintenances");

            migrationBuilder.DropIndex(
                name: "IX_LicenseApplications_GovernorateId",
                table: "LicenseApplications");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_GovernorateId",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_FloatingUnitStaffs_GovernorateId",
                table: "FloatingUnitStaffs");

            migrationBuilder.DropIndex(
                name: "IX_FloatingUnits_GovernorateId",
                table: "FloatingUnits");

            migrationBuilder.DropIndex(
                name: "IX_FloatingUnitOrganizations_GovernorateId",
                table: "FloatingUnitOrganizations");

            migrationBuilder.DropIndex(
                name: "IX_Accidents_GovernorateId",
                table: "Accidents");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "TripInformations");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "TripGeos");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "TouristMarinas");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "OrganizationStaffs");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "NotificationGroups");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "NationalityTrips");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "MessagingGroups");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "MarinaTrips");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "MarinaOrganizations");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "LicenseApplications");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "FloatingUnitStaffs");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "FloatingUnits");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "FloatingUnitOrganizations");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "Accidents");
        }
    }
}
