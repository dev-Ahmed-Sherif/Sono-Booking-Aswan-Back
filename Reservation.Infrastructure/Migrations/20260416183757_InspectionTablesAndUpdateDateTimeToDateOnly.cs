using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InspectionTablesAndUpdateDateTimeToDateOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inspections_TripInformations_TripInformationId",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_TripInformationId",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "IsInspected",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "TripInformationId",
                table: "Inspections");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "SartDate",
                table: "TripInformations",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EndDate",
                table: "TripInformations",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "LicenseDate",
                table: "TouristMarinaLicenseApplications",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "Inspections",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "InspectionDate",
                table: "Inspections",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "FloatingUnitId",
                table: "Inspections",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "InspectionClauses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InspectionTypeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionClauses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionClauses_InspectionClauses_ParentId",
                        column: x => x.ParentId,
                        principalTable: "InspectionClauses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InspectionClauses_InspectionTypes_InspectionTypeId",
                        column: x => x.InspectionTypeId,
                        principalTable: "InspectionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InspectionFloatingUnitClauses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsInspected = table.Column<bool>(type: "bit", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InspectionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InspectionClauseId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionFloatingUnitClauses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionFloatingUnitClauses_InspectionClauses_InspectionClauseId",
                        column: x => x.InspectionClauseId,
                        principalTable: "InspectionClauses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InspectionFloatingUnitClauses_Inspections_InspectionId",
                        column: x => x.InspectionId,
                        principalTable: "Inspections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionClauses_InspectionTypeId",
                table: "InspectionClauses",
                column: "InspectionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionClauses_ParentId",
                table: "InspectionClauses",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionFloatingUnitClauses_InspectionClauseId",
                table: "InspectionFloatingUnitClauses",
                column: "InspectionClauseId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionFloatingUnitClauses_InspectionId",
                table: "InspectionFloatingUnitClauses",
                column: "InspectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InspectionFloatingUnitClauses");

            migrationBuilder.DropTable(
                name: "InspectionClauses");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SartDate",
                table: "TripInformations",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "TripInformations",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LicenseDate",
                table: "TouristMarinaLicenseApplications",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "Inspections",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "InspectionDate",
                table: "Inspections",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "FloatingUnitId",
                table: "Inspections",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<bool>(
                name: "IsInspected",
                table: "Inspections",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TripInformationId",
                table: "Inspections",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_TripInformationId",
                table: "Inspections",
                column: "TripInformationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inspections_TripInformations_TripInformationId",
                table: "Inspections",
                column: "TripInformationId",
                principalTable: "TripInformations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
