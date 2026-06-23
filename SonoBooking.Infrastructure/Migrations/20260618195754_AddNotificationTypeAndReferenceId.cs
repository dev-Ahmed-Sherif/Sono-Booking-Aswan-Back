using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationTypeAndReferenceId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferenceId",
                table: "Notifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Notifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "system");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_MessagingGroup_MessagingGroupId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_MessagingGroup_Governorate_GovernorateId",
                table: "MessagingGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessagingGroup",
                table: "MessagingGroup");

            migrationBuilder.RenameTable(
                name: "MessagingGroup",
                newName: "MessagingGroups");

            migrationBuilder.RenameIndex(
                name: "IX_MessagingGroup_GovernorateId",
                table: "MessagingGroups",
                newName: "IX_MessagingGroups_GovernorateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessagingGroups",
                table: "MessagingGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_MessagingGroups_MessagingGroupId",
                table: "Messages",
                column: "MessagingGroupId",
                principalTable: "MessagingGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MessagingGroups_Governorate_GovernorateId",
                table: "MessagingGroups",
                column: "GovernorateId",
                principalTable: "Governorate",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_MessagingGroups_MessagingGroupId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_MessagingGroups_Governorate_GovernorateId",
                table: "MessagingGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessagingGroups",
                table: "MessagingGroups");

            migrationBuilder.RenameTable(
                name: "MessagingGroups",
                newName: "MessagingGroup");

            migrationBuilder.RenameIndex(
                name: "IX_MessagingGroups_GovernorateId",
                table: "MessagingGroup",
                newName: "IX_MessagingGroup_GovernorateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessagingGroup",
                table: "MessagingGroup",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_MessagingGroup_MessagingGroupId",
                table: "Messages",
                column: "MessagingGroupId",
                principalTable: "MessagingGroup",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MessagingGroup_Governorate_GovernorateId",
                table: "MessagingGroup",
                column: "GovernorateId",
                principalTable: "Governorate",
                principalColumn: "Id");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Notifications");
        }
    }
}
