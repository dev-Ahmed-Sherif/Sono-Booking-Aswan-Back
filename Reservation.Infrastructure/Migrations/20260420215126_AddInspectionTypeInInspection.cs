using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInspectionTypeInInspection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inspections_InspectionTypes_InspectionTypeId",
                table: "Inspections");

            migrationBuilder.AlterColumn<string>(
                name: "InspectionTypeId",
                table: "Inspections",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Inspections_InspectionTypes_InspectionTypeId",
                table: "Inspections",
                column: "InspectionTypeId",
                principalTable: "InspectionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inspections_InspectionTypes_InspectionTypeId",
                table: "Inspections");

            migrationBuilder.AlterColumn<string>(
                name: "InspectionTypeId",
                table: "Inspections",
                type: "nvarchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddForeignKey(
                name: "FK_Inspections_InspectionTypes_InspectionTypeId",
                table: "Inspections",
                column: "InspectionTypeId",
                principalTable: "InspectionTypes",
                principalColumn: "Id");
        }
    }
}
