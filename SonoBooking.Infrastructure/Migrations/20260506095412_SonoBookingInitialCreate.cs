using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SonoBookingInitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "units");

            migrationBuilder.EnsureSchema(
                name: "booking");

            migrationBuilder.CreateTable(
                name: "ApartmentType",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApartmentType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Extension = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Attachme__3214EC074E4C76EA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Governorate",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Governorate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Leaders",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Position = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Leaders__3214EC070064AA06", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Relationships",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relationships", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestType",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoomType",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    NationalIdUrl = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: true),
                    DocumentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DocumentType = table.Column<int>(type: "int", nullable: true),
                    IsLogedIn = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: true),
                    GovernorateId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", x => x.Id);
                    table.ForeignKey(
                        name: "FK_City_Governorate_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "Governorate",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MessagingGroup",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GovernorateId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessagingGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessagingGroup_Governorate_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "Governorate",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NotificationGroup",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GovernorateId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationGroup_Governorate_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "Governorate",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Companions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DocumentImageUrl = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RelationshipId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Companio__3214EC0728FDF18A", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companions_Relationship",
                        column: x => x.RelationshipId,
                        principalTable: "Relationships",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Companions_Users",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Token = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequestTypeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApprovedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    LeaderId = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Requests__3214EC07B2D76077", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requests_Leaders_LeaderId",
                        column: x => x.LeaderId,
                        principalSchema: "booking",
                        principalTable: "Leaders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Requests_RequestType_RequestTypeId",
                        column: x => x.RequestTypeId,
                        principalTable: "RequestType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Requests_User_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Requests_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Apartments",
                schema: "units",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApartmentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Available"),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "Male"),
                    AllocationType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "Fixed"),
                    Street = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BuildingNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Floor = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DetailedAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ApartmentTypeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GovernorateId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CityId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Apartmen__3214EC0721231588", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Apartments_ApartmentType_ApartmentTypeId",
                        column: x => x.ApartmentTypeId,
                        principalTable: "ApartmentType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Apartments_City_CityId",
                        column: x => x.CityId,
                        principalTable: "City",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Apartments_Governorate_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "Governorate",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Town",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CityId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Town", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Town_City_CityId",
                        column: x => x.CityId,
                        principalTable: "City",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 14000, nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReceiverId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MessagingGroupId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GovernorateId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Governorate_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "Governorate",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Messages_MessagingGroup_MessagingGroupId",
                        column: x => x.MessagingGroupId,
                        principalTable: "MessagingGroup",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Messages_User_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_User_SenderId",
                        column: x => x.SenderId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 14000, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReceiverId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NotificationGroupId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GovernorateId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Governorate_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "Governorate",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notifications_NotificationGroup_NotificationGroupId",
                        column: x => x.NotificationGroupId,
                        principalTable: "NotificationGroup",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notifications_User_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_User_SenderId",
                        column: x => x.SenderId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Approvals",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LeaderId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Decision = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DecisionDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Approval__3214EC0734F8A54C", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Approvals_Leader",
                        column: x => x.LeaderId,
                        principalSchema: "booking",
                        principalTable: "Leaders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Approvals_Request",
                        column: x => x.RequestId,
                        principalSchema: "booking",
                        principalTable: "Requests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RequestParticipants",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RequestP__3214EC07565E9385", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestParticipants_Companion",
                        column: x => x.CompanionId,
                        principalTable: "Companions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestParticipants_Request",
                        column: x => x.RequestId,
                        principalSchema: "booking",
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                schema: "units",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RoomNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Available"),
                    ApartmentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RoomTypeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Rooms__3214EC07E6B4CB77", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Apartments",
                        column: x => x.ApartmentId,
                        principalSchema: "units",
                        principalTable: "Apartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rooms_RoomType_RoomTypeId",
                        column: x => x.RoomTypeId,
                        principalTable: "RoomType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Beds",
                schema: "units",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BedNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Dimensions = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Available"),
                    RoomId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Beds__3214EC079A1B9CD4", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beds_Rooms",
                        column: x => x.RoomId,
                        principalSchema: "units",
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestUnits",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApartmentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BedId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RoomId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RequestU__3214EC07A33A8588", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestUnits_Apartment",
                        column: x => x.ApartmentId,
                        principalSchema: "units",
                        principalTable: "Apartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestUnits_Bed",
                        column: x => x.BedId,
                        principalSchema: "units",
                        principalTable: "Beds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RequestUnits_Request",
                        column: x => x.RequestId,
                        principalSchema: "booking",
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestUnits_Room",
                        column: x => x.RoomId,
                        principalSchema: "units",
                        principalTable: "Rooms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CheckInDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ActualCheckOutDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Reserved"),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApartmentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RoomId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BedId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reservat__3214EC07909291A0", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_Apartment",
                        column: x => x.ApartmentId,
                        principalSchema: "units",
                        principalTable: "Apartments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reservations_Bed",
                        column: x => x.BedId,
                        principalSchema: "units",
                        principalTable: "Beds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reservations_Request",
                        column: x => x.RequestId,
                        principalSchema: "booking",
                        principalTable: "Requests",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reservations_Room",
                        column: x => x.RoomId,
                        principalSchema: "units",
                        principalTable: "Rooms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reservations_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UnitImages",
                schema: "units",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApartmentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RoomId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BedId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AttachmentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UnitImag__3214EC074E4C76EA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitImages_Apartment",
                        column: x => x.ApartmentId,
                        principalSchema: "units",
                        principalTable: "Apartments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UnitImages_Attachments_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitImages_Bed",
                        column: x => x.BedId,
                        principalSchema: "units",
                        principalTable: "Beds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UnitImages_Room",
                        column: x => x.RoomId,
                        principalSchema: "units",
                        principalTable: "Rooms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    PaymentDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReservationId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    CreatedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payments__3214EC0797960E75", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Reservation",
                        column: x => x.ReservationId,
                        principalSchema: "booking",
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IDX_Apartments_Gender",
                schema: "units",
                table: "Apartments",
                column: "Gender");

            migrationBuilder.CreateIndex(
                name: "IDX_Apartments_Status",
                schema: "units",
                table: "Apartments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_ApartmentTypeId",
                schema: "units",
                table: "Apartments",
                column: "ApartmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_CityId",
                schema: "units",
                table: "Apartments",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_GovernorateId",
                schema: "units",
                table: "Apartments",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "UX_Apartment_Number_Location",
                schema: "units",
                table: "Apartments",
                columns: new[] { "ApartmentNumber", "BuildingNumber", "Street", "CityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_LeaderId",
                schema: "booking",
                table: "Approvals",
                column: "LeaderId");

            migrationBuilder.CreateIndex(
                name: "UX_Approvals_Request",
                schema: "booking",
                table: "Approvals",
                column: "RequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_Beds_RoomId",
                schema: "units",
                table: "Beds",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IDX_Beds_Status",
                schema: "units",
                table: "Beds",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "UX_Bed_Number_Per_Room",
                schema: "units",
                table: "Beds",
                columns: new[] { "RoomId", "BedNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_City_GovernorateId",
                table: "City",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_Companions_RelationshipId",
                table: "Companions",
                column: "RelationshipId");

            migrationBuilder.CreateIndex(
                name: "IX_Companions_UserId",
                table: "Companions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UX_Companions_DocumentNumber",
                table: "Companions",
                column: "DocumentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_Leaders_IsActive",
                schema: "booking",
                table: "Leaders",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "UX_Leaders_Name_Position",
                schema: "booking",
                table: "Leaders",
                columns: new[] { "FullName", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_GovernorateId",
                table: "Messages",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MessagingGroupId",
                table: "Messages",
                column: "MessagingGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverId",
                table: "Messages",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserId",
                table: "Messages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessagingGroup_GovernorateId",
                table: "MessagingGroup",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationGroup_GovernorateId",
                table: "NotificationGroup",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_GovernorateId",
                table: "Notifications",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NotificationGroupId",
                table: "Notifications",
                column: "NotificationGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ReceiverId",
                table: "Notifications",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SenderId",
                table: "Notifications",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IDX_Payments_ReservationId",
                schema: "booking",
                table: "Payments",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IDX_Payments_Status",
                schema: "booking",
                table: "Payments",
                column: "PaymentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IDX_RequestParticipants_RequestId",
                schema: "booking",
                table: "RequestParticipants",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestParticipants_CompanionId",
                schema: "booking",
                table: "RequestParticipants",
                column: "CompanionId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ApprovedById",
                schema: "booking",
                table: "Requests",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_LeaderId",
                schema: "booking",
                table: "Requests",
                column: "LeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequestTypeId",
                schema: "booking",
                table: "Requests",
                column: "RequestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_UserId",
                schema: "booking",
                table: "Requests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UX_Request_Number",
                schema: "booking",
                table: "Requests",
                column: "RequestNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_RequestUnits_RequestId",
                schema: "booking",
                table: "RequestUnits",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestUnits_ApartmentId",
                schema: "booking",
                table: "RequestUnits",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestUnits_BedId",
                schema: "booking",
                table: "RequestUnits",
                column: "BedId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestUnits_RoomId",
                schema: "booking",
                table: "RequestUnits",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IDX_Reservations_Dates",
                schema: "booking",
                table: "Reservations",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IDX_Reservations_RequestId",
                schema: "booking",
                table: "Reservations",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IDX_Reservations_Status",
                schema: "booking",
                table: "Reservations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IDX_Reservations_UserId",
                schema: "booking",
                table: "Reservations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ApartmentId",
                schema: "booking",
                table: "Reservations",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_BedId",
                schema: "booking",
                table: "Reservations",
                column: "BedId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_RoomId",
                schema: "booking",
                table: "Reservations",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IDX_Rooms_ApartmentId",
                schema: "units",
                table: "Rooms",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IDX_Rooms_Status",
                schema: "units",
                table: "Rooms",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomTypeId",
                schema: "units",
                table: "Rooms",
                column: "RoomTypeId");

            migrationBuilder.CreateIndex(
                name: "UX_Room_Number_Per_Apartment",
                schema: "units",
                table: "Rooms",
                columns: new[] { "ApartmentId", "RoomNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Town_CityId",
                table: "Town",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IDX_UnitImages_ApartmentId",
                schema: "units",
                table: "UnitImages",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IDX_UnitImages_BedId",
                schema: "units",
                table: "UnitImages",
                column: "BedId");

            migrationBuilder.CreateIndex(
                name: "IDX_UnitImages_RoomId",
                schema: "units",
                table: "UnitImages",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitImages_AttachmentId",
                schema: "units",
                table: "UnitImages",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "UX_UnitImages_Unique",
                schema: "units",
                table: "UnitImages",
                columns: new[] { "ApartmentId", "RoomId", "BedId", "AttachmentId" },
                unique: true,
                filter: "[ApartmentId] IS NOT NULL AND [RoomId] IS NOT NULL AND [BedId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Approvals",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Payments",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RequestParticipants",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "RequestUnits",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "Town");

            migrationBuilder.DropTable(
                name: "UnitImages",
                schema: "units");

            migrationBuilder.DropTable(
                name: "MessagingGroup");

            migrationBuilder.DropTable(
                name: "NotificationGroup");

            migrationBuilder.DropTable(
                name: "Reservations",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "Companions");

            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "Beds",
                schema: "units");

            migrationBuilder.DropTable(
                name: "Requests",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "Relationships");

            migrationBuilder.DropTable(
                name: "Rooms",
                schema: "units");

            migrationBuilder.DropTable(
                name: "Leaders",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "RequestType");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Apartments",
                schema: "units");

            migrationBuilder.DropTable(
                name: "RoomType");

            migrationBuilder.DropTable(
                name: "ApartmentType");

            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropTable(
                name: "Governorate");
        }
    }
}
