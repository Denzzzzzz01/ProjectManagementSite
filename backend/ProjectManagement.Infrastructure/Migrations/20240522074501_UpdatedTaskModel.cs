using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProjectManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTaskModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_AppUserId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_AppUserId",
                table: "Projects");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("a5eb87f4-a1a2-4007-8a53-5e00aac79364"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("eec0105b-54a6-47ad-b326-42d919036fa0"));

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "priority",
                table: "Tasks",
                newName: "Priority");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("29cb6d53-c871-42c1-ba04-5a7bdc934eac"), null, "Admin", "ADMIN" },
                    { new Guid("ebb5b1be-d8c6-4b67-9fdc-99fc914d05f5"), null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("29cb6d53-c871-42c1-ba04-5a7bdc934eac"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("ebb5b1be-d8c6-4b67-9fdc-99fc914d05f5"));

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "Tasks",
                newName: "priority");

            migrationBuilder.AddColumn<Guid>(
                name: "AppUserId",
                table: "Projects",
                type: "uuid",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("a5eb87f4-a1a2-4007-8a53-5e00aac79364"), null, "Admin", "ADMIN" },
                    { new Guid("eec0105b-54a6-47ad-b326-42d919036fa0"), null, "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_AppUserId",
                table: "Projects",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_AppUserId",
                table: "Projects",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
