using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachemntToAccidentInspectionMaintenance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accidents_AccidentTypes_AccidentTypeId",
                table: "Accidents");

            migrationBuilder.DropForeignKey(
                name: "FK_Accidents_Organizations_OrganizationId",
                table: "Accidents");

            migrationBuilder.DropIndex(
                name: "IX_Accidents_OrganizationId",
                table: "Accidents");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Accidents");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "FloatingUnitOrganizations",
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
                table: "FloatingUnitOrganizations",
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
                table: "Accidents",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccidentTypeId",
                table: "Accidents",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "AccidentAttachments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AttachmentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccidentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccidentAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccidentAttachments_Accidents_AccidentId",
                        column: x => x.AccidentId,
                        principalTable: "Accidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccidentAttachments_Attachments_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccidentOrganizations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccidentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccidentOrganizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccidentOrganizations_Accidents_AccidentId",
                        column: x => x.AccidentId,
                        principalTable: "Accidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccidentOrganizations_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InspectionAttachments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AttachmentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InspectionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionAttachments_Attachments_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InspectionAttachments_Inspections_InspectionId",
                        column: x => x.InspectionId,
                        principalTable: "Inspections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceAttachments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AttachmentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaintenanceId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceAttachments_Attachments_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaintenanceAttachments_Maintenances_MaintenanceId",
                        column: x => x.MaintenanceId,
                        principalTable: "Maintenances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccidentAttachments_AccidentId",
                table: "AccidentAttachments",
                column: "AccidentId");

            migrationBuilder.CreateIndex(
                name: "IX_AccidentAttachments_AttachmentId",
                table: "AccidentAttachments",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AccidentOrganizations_AccidentId",
                table: "AccidentOrganizations",
                column: "AccidentId");

            migrationBuilder.CreateIndex(
                name: "IX_AccidentOrganizations_OrganizationId",
                table: "AccidentOrganizations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionAttachments_AttachmentId",
                table: "InspectionAttachments",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionAttachments_InspectionId",
                table: "InspectionAttachments",
                column: "InspectionId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceAttachments_AttachmentId",
                table: "MaintenanceAttachments",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceAttachments_MaintenanceId",
                table: "MaintenanceAttachments",
                column: "MaintenanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accidents_AccidentTypes_AccidentTypeId",
                table: "Accidents",
                column: "AccidentTypeId",
                principalTable: "AccidentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accidents_AccidentTypes_AccidentTypeId",
                table: "Accidents");

            migrationBuilder.DropTable(
                name: "AccidentAttachments");

            migrationBuilder.DropTable(
                name: "AccidentOrganizations");

            migrationBuilder.DropTable(
                name: "InspectionAttachments");

            migrationBuilder.DropTable(
                name: "MaintenanceAttachments");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "FloatingUnitOrganizations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FloatingUnitId",
                table: "FloatingUnitOrganizations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FloatingUnitId",
                table: "Accidents",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "AccidentTypeId",
                table: "Accidents",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "Accidents",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accidents_OrganizationId",
                table: "Accidents",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accidents_AccidentTypes_AccidentTypeId",
                table: "Accidents",
                column: "AccidentTypeId",
                principalTable: "AccidentTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Accidents_Organizations_OrganizationId",
                table: "Accidents",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
