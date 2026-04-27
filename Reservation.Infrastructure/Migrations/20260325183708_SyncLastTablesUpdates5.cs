using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncLastTablesUpdates5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_MaintenanceTypes_MaintenanceTypeId",
                table: "Maintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_Governorates_GovernorateId",
                table: "Organizations");

            migrationBuilder.DropForeignKey(
                name: "FK_TouristMarinas_Towns_TownId",
                table: "TouristMarinas");

            migrationBuilder.DropForeignKey(
                name: "FK_TripInformations_Governorates_GovernorateId",
                table: "TripInformations");

            migrationBuilder.AddColumn<string>(
                name: "CityId",
                table: "TouristMarinas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "MaintenanceTypeId",
                table: "Maintenances",
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
                table: "Maintenances",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityId",
                table: "Accidents",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TouristMarinas_CityId",
                table: "TouristMarinas",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Accidents_CityId",
                table: "Accidents",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accidents_Cities_CityId",
                table: "Accidents",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_MaintenanceTypes_MaintenanceTypeId",
                table: "Maintenances",
                column: "MaintenanceTypeId",
                principalTable: "MaintenanceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_Governorates_GovernorateId",
                table: "Organizations",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TouristMarinas_Cities_CityId",
                table: "TouristMarinas",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TouristMarinas_Towns_TownId",
                table: "TouristMarinas",
                column: "TownId",
                principalTable: "Towns",
                principalColumn: "Id");

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
                name: "FK_Accidents_Cities_CityId",
                table: "Accidents");

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_MaintenanceTypes_MaintenanceTypeId",
                table: "Maintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_Governorates_GovernorateId",
                table: "Organizations");

            migrationBuilder.DropForeignKey(
                name: "FK_TouristMarinas_Cities_CityId",
                table: "TouristMarinas");

            migrationBuilder.DropForeignKey(
                name: "FK_TouristMarinas_Towns_TownId",
                table: "TouristMarinas");

            migrationBuilder.DropForeignKey(
                name: "FK_TripInformations_Governorates_GovernorateId",
                table: "TripInformations");

            migrationBuilder.DropIndex(
                name: "IX_TouristMarinas_CityId",
                table: "TouristMarinas");

            migrationBuilder.DropIndex(
                name: "IX_Accidents_CityId",
                table: "Accidents");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "TouristMarinas");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Accidents");

            migrationBuilder.AlterColumn<string>(
                name: "MaintenanceTypeId",
                table: "Maintenances",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FloatingUnitId",
                table: "Maintenances",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_MaintenanceTypes_MaintenanceTypeId",
                table: "Maintenances",
                column: "MaintenanceTypeId",
                principalTable: "MaintenanceTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_Governorates_GovernorateId",
                table: "Organizations",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TouristMarinas_Towns_TownId",
                table: "TouristMarinas",
                column: "TownId",
                principalTable: "Towns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TripInformations_Governorates_GovernorateId",
                table: "TripInformations",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id");
        }
    }
}
