using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInspectionClauseRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GovernorateId",
                table: "InspectionClauses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernorateId",
                table: "GeoPoints",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InspectionClauses_GovernorateId",
                table: "InspectionClauses",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_GeoPoints_GovernorateId",
                table: "GeoPoints",
                column: "GovernorateId");

            migrationBuilder.AddForeignKey(
                name: "FK_GeoPoints_Governorates_GovernorateId",
                table: "GeoPoints",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionClauses_Governorates_GovernorateId",
                table: "InspectionClauses",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GeoPoints_Governorates_GovernorateId",
                table: "GeoPoints");

            migrationBuilder.DropForeignKey(
                name: "FK_InspectionClauses_Governorates_GovernorateId",
                table: "InspectionClauses");

            migrationBuilder.DropIndex(
                name: "IX_InspectionClauses_GovernorateId",
                table: "InspectionClauses");

            migrationBuilder.DropIndex(
                name: "IX_GeoPoints_GovernorateId",
                table: "GeoPoints");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "InspectionClauses");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "GeoPoints");
        }
    }
}
