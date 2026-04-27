using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncLastTablesUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_InspectionTypes_InspectionTypeId",
                table: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_InspectionTypeId",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "InspectionTypeId",
                table: "Organizations");

            migrationBuilder.AddColumn<string>(
                name: "TouristMarinaNumber",
                table: "Organizations",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TouristMarinaNumber",
                table: "Organizations");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Organizations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InspectionTypeId",
                table: "Organizations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_InspectionTypeId",
                table: "Organizations",
                column: "InspectionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_InspectionTypes_InspectionTypeId",
                table: "Organizations",
                column: "InspectionTypeId",
                principalTable: "InspectionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
