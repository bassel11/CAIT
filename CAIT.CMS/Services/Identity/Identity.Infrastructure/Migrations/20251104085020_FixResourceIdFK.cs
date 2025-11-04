using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixResourceIdFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionAssignments_Resources_ResourceId1",
                table: "UserPermissionAssignments");

            migrationBuilder.DropIndex(
                name: "IX_UserPermissionAssignments_ResourceId1",
                table: "UserPermissionAssignments");

            migrationBuilder.DropColumn(
                name: "ResourceId1",
                table: "UserPermissionAssignments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ResourceId1",
                table: "UserPermissionAssignments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionAssignments_ResourceId1",
                table: "UserPermissionAssignments",
                column: "ResourceId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionAssignments_Resources_ResourceId1",
                table: "UserPermissionAssignments",
                column: "ResourceId1",
                principalTable: "Resources",
                principalColumn: "Id");
        }
    }
}
