using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDbContextRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inspections_InspectionTypes_InspectionTypeId1",
                table: "Inspections");

            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_InspectionTypes_InspectionTypeId",
                table: "Organizations");

            migrationBuilder.DropForeignKey(
                name: "FK_TouristMarinas_GeoPoints_GeoPointId",
                table: "TouristMarinas");

            migrationBuilder.DropForeignKey(
                name: "FK_TouristMarinas_Towns_TownId",
                table: "TouristMarinas");

            migrationBuilder.DropForeignKey(
                name: "FK_TripGeos_GeoPoints_GeoPointId",
                table: "TripGeos");

            migrationBuilder.DropForeignKey(
                name: "FK_TripInformations_Routes_RouteId",
                table: "TripInformations");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_InspectionTypeId1",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "InspectionTypeId1",
                table: "Inspections");

            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "Maintenances",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InspectionTypeId",
                table: "Inspections",
                type: "nvarchar(50)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_OrganizationId",
                table: "Maintenances",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_InspectionTypeId",
                table: "Inspections",
                column: "InspectionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inspections_InspectionTypes_InspectionTypeId",
                table: "Inspections",
                column: "InspectionTypeId",
                principalTable: "InspectionTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_Organizations_OrganizationId",
                table: "Maintenances",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_InspectionTypes_InspectionTypeId",
                table: "Organizations",
                column: "InspectionTypeId",
                principalTable: "InspectionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TouristMarinas_GeoPoints_GeoPointId",
                table: "TouristMarinas",
                column: "GeoPointId",
                principalTable: "GeoPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TouristMarinas_Towns_TownId",
                table: "TouristMarinas",
                column: "TownId",
                principalTable: "Towns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TripGeos_GeoPoints_GeoPointId",
                table: "TripGeos",
                column: "GeoPointId",
                principalTable: "GeoPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TripInformations_Routes_RouteId",
                table: "TripInformations",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inspections_InspectionTypes_InspectionTypeId",
                table: "Inspections");

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Organizations_OrganizationId",
                table: "Maintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_InspectionTypes_InspectionTypeId",
                table: "Organizations");

            migrationBuilder.DropForeignKey(
                name: "FK_TouristMarinas_GeoPoints_GeoPointId",
                table: "TouristMarinas");

            migrationBuilder.DropForeignKey(
                name: "FK_TouristMarinas_Towns_TownId",
                table: "TouristMarinas");

            migrationBuilder.DropForeignKey(
                name: "FK_TripGeos_GeoPoints_GeoPointId",
                table: "TripGeos");

            migrationBuilder.DropForeignKey(
                name: "FK_TripInformations_Routes_RouteId",
                table: "TripInformations");

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_OrganizationId",
                table: "Maintenances");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_InspectionTypeId",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Maintenances");

            migrationBuilder.AlterColumn<int>(
                name: "InspectionTypeId",
                table: "Inspections",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InspectionTypeId1",
                table: "Inspections",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_InspectionTypeId1",
                table: "Inspections",
                column: "InspectionTypeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Inspections_InspectionTypes_InspectionTypeId1",
                table: "Inspections",
                column: "InspectionTypeId1",
                principalTable: "InspectionTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_InspectionTypes_InspectionTypeId",
                table: "Organizations",
                column: "InspectionTypeId",
                principalTable: "InspectionTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TouristMarinas_GeoPoints_GeoPointId",
                table: "TouristMarinas",
                column: "GeoPointId",
                principalTable: "GeoPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TouristMarinas_Towns_TownId",
                table: "TouristMarinas",
                column: "TownId",
                principalTable: "Towns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TripGeos_GeoPoints_GeoPointId",
                table: "TripGeos",
                column: "GeoPointId",
                principalTable: "GeoPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TripInformations_Routes_RouteId",
                table: "TripInformations",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
