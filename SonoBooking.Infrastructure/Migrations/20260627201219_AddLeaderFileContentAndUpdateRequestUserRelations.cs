using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonoBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaderFileContentAndUpdateRequestUserRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestToId",
                schema: "booking",
                table: "Requests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE [booking].[Requests]
                SET [RequestToId] = [LeaderId]
                WHERE [LeaderId] IS NOT NULL
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Leaders_LeaderId",
                schema: "booking",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_LeaderId",
                schema: "booking",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "LeaderId",
                schema: "booking",
                table: "Requests");

            migrationBuilder.AddColumn<byte[]>(
                name: "FileContent",
                schema: "booking",
                table: "Leaders",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeaderId",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Percentage",
                schema: "booking",
                table: "Requests",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.Sql("""
                UPDATE [booking].[Requests]
                SET [RequestToId] = (
                    SELECT TOP 1 [Id]
                    FROM [booking].[Leaders]
                    ORDER BY [CreatedAt]
                )
                WHERE ([RequestToId] IS NULL OR [RequestToId] = '')
                  AND EXISTS (SELECT 1 FROM [booking].[Leaders])
                """);

            migrationBuilder.AlterColumn<string>(
                name: "RequestToId",
                schema: "booking",
                table: "Requests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequestToId",
                schema: "booking",
                table: "Requests",
                column: "RequestToId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LeaderId",
                table: "AspNetUsers",
                column: "LeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Leaders_LeaderId",
                table: "AspNetUsers",
                column: "LeaderId",
                principalSchema: "booking",
                principalTable: "Leaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Leaders_RequestToId",
                schema: "booking",
                table: "Requests",
                column: "RequestToId",
                principalSchema: "booking",
                principalTable: "Leaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Leaders_LeaderId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Leaders_RequestToId",
                schema: "booking",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_RequestToId",
                schema: "booking",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LeaderId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "RequestToId",
                schema: "booking",
                table: "Requests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.DropColumn(
                name: "FileContent",
                schema: "booking",
                table: "Leaders");

            migrationBuilder.DropColumn(
                name: "LeaderId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Percentage",
                schema: "booking",
                table: "Requests");

            migrationBuilder.AddColumn<string>(
                name: "LeaderId",
                schema: "booking",
                table: "Requests",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE [booking].[Requests]
                SET [LeaderId] = [RequestToId]
                WHERE [RequestToId] IS NOT NULL
                """);

            migrationBuilder.DropColumn(
                name: "RequestToId",
                schema: "booking",
                table: "Requests");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_LeaderId",
                schema: "booking",
                table: "Requests",
                column: "LeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Leaders_LeaderId",
                schema: "booking",
                table: "Requests",
                column: "LeaderId",
                principalSchema: "booking",
                principalTable: "Leaders",
                principalColumn: "Id");
        }
    }
}
