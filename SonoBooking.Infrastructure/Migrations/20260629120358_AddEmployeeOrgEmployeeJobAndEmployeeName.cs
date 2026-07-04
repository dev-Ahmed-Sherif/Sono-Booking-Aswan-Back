using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeOrgEmployeeJobAndEmployeeName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmployeeJobId",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeOrgId",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Employees",
                type: "nvarchar(280)",
                maxLength: 280,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "EmployeeJobs",
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
                    table.PrimaryKey("PK_EmployeeJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeOrgs",
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
                    table.PrimaryKey("PK_EmployeeOrgs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeJobId",
                table: "Employees",
                column: "EmployeeJobId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeOrgId",
                table: "Employees",
                column: "EmployeeOrgId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_EmployeeJob",
                table: "Employees",
                column: "EmployeeJobId",
                principalTable: "EmployeeJobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_EmployeeOrg",
                table: "Employees",
                column: "EmployeeOrgId",
                principalTable: "EmployeeOrgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_EmployeeJob",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_EmployeeOrg",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "EmployeeJobs");

            migrationBuilder.DropTable(
                name: "EmployeeOrgs");

            migrationBuilder.DropIndex(
                name: "IX_Employees_EmployeeJobId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_EmployeeOrgId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EmployeeJobId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EmployeeOrgId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Employees");
        }
    }
}
