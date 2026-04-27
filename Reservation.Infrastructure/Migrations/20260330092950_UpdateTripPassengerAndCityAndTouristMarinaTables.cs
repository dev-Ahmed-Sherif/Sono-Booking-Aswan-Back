using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTripPassengerAndCityAndTouristMarinaTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentDisplaySize",
                table: "TripPassengerAttachments");

            migrationBuilder.DropColumn(
                name: "Extension",
                table: "TripPassengerAttachments");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "TripPassengerAttachments");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "TripPassengerAttachments");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "TripPassengerAttachments");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "TripPassengerAttachments");

            migrationBuilder.DropColumn(
                name: "PassengerAttachment",
                table: "TripInformations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Cities");

            migrationBuilder.RenameColumn(
                name: "FileId",
                table: "TripPassengerAttachments",
                newName: "AttachmentId");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "TouristMarinas",
                newName: "MarinaAddress");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "TouristMarinas",
                newName: "ImageUrl");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "TripInformations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "AttachmentNumber",
                table: "TripInformations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GeoPointId1",
                table: "TripGeos",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "TouristMarinas",
                type: "nvarchar(35)",
                maxLength: 35,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "TouristMarinas",
                type: "nvarchar(280)",
                maxLength: 280,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "TouristMarinas",
                type: "nvarchar(280)",
                maxLength: 280,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TouristMarinaId",
                table: "TouristMarinaOrganizations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ToDate",
                table: "TouristMarinaOrganizations",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "TouristMarinaOrganizations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LicenseNumber",
                table: "TouristMarinaOrganizations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "FromDate",
                table: "TouristMarinaOrganizations",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateTable(
                name: "Attachment",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    AttachmentDisplaySize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachment", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TripPassengerAttachments_AttachmentId",
                table: "TripPassengerAttachments",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TripGeos_GeoPointId1",
                table: "TripGeos",
                column: "GeoPointId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TripGeos_GeoPoints_GeoPointId1",
                table: "TripGeos",
                column: "GeoPointId1",
                principalTable: "GeoPoints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TripPassengerAttachments_Attachment_AttachmentId",
                table: "TripPassengerAttachments",
                column: "AttachmentId",
                principalTable: "Attachment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripGeos_GeoPoints_GeoPointId1",
                table: "TripGeos");

            migrationBuilder.DropForeignKey(
                name: "FK_TripPassengerAttachments_Attachment_AttachmentId",
                table: "TripPassengerAttachments");

            migrationBuilder.DropTable(
                name: "Attachment");

            migrationBuilder.DropIndex(
                name: "IX_TripPassengerAttachments_AttachmentId",
                table: "TripPassengerAttachments");

            migrationBuilder.DropIndex(
                name: "IX_TripGeos_GeoPointId1",
                table: "TripGeos");

            migrationBuilder.DropColumn(
                name: "AttachmentNumber",
                table: "TripInformations");

            migrationBuilder.DropColumn(
                name: "GeoPointId1",
                table: "TripGeos");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "TouristMarinas");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "TouristMarinas");

            migrationBuilder.RenameColumn(
                name: "AttachmentId",
                table: "TripPassengerAttachments",
                newName: "FileId");

            migrationBuilder.RenameColumn(
                name: "MarinaAddress",
                table: "TouristMarinas",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "TouristMarinas",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "AttachmentDisplaySize",
                table: "TripPassengerAttachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "TripPassengerAttachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "TripPassengerAttachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "TripPassengerAttachments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "TripPassengerAttachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "TripPassengerAttachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "TripInformations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "PassengerAttachment",
                table: "TripInformations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "TouristMarinas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(35)",
                oldMaxLength: 35);

            migrationBuilder.AlterColumn<string>(
                name: "TouristMarinaId",
                table: "TouristMarinaOrganizations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ToDate",
                table: "TouristMarinaOrganizations",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "TouristMarinaOrganizations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "LicenseNumber",
                table: "TouristMarinaOrganizations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FromDate",
                table: "TouristMarinaOrganizations",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Cities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Cities",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Cities",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "Cities",
                type: "nvarchar(28)",
                maxLength: 28,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Cities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Cities",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "Cities",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
