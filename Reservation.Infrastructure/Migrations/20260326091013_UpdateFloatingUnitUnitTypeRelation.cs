using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFloatingUnitUnitTypeRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FloatingUnits_UnitTypes_UnitTypeId",
                table: "FloatingUnits");

            migrationBuilder.AddForeignKey(
                name: "FK_FloatingUnits_UnitTypes_UnitTypeId",
                table: "FloatingUnits",
                column: "UnitTypeId",
                principalTable: "UnitTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FloatingUnits_UnitTypes_UnitTypeId",
                table: "FloatingUnits");

            migrationBuilder.AddForeignKey(
                name: "FK_FloatingUnits_UnitTypes_UnitTypeId",
                table: "FloatingUnits",
                column: "UnitTypeId",
                principalTable: "UnitTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
