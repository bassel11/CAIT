using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetIdentityServiceForGlobalandReusable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Resources_ResourceId",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_UserPerm_User_Committee",
                table: "UserPermissionAssignments");

            migrationBuilder.DropIndex(
                name: "IX_UserPerm_User_Permission",
                table: "UserPermissionAssignments");

            migrationBuilder.DropIndex(
                name: "IX_UserPerm_User_Resource",
                table: "UserPermissionAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_Committee",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_Resources_Committee",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "CommitteeId",
                table: "UserPermissionAssignments");

            migrationBuilder.DropColumn(
                name: "CommitteeId",
                table: "RolePermissions");

            migrationBuilder.RenameColumn(
                name: "CommitteeId",
                table: "Resources",
                newName: "ParentReferenceId");

            migrationBuilder.RenameIndex(
                name: "UX_Resources_Type_Ref",
                table: "Resources",
                newName: "UX_Resources_Type_ExternalRef");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResourceId",
                table: "RolePermissions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentResourceType",
                table: "Resources",
                type: "int",
                maxLength: 100,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId", "ScopeType", "ResourceId" });

            migrationBuilder.CreateIndex(
                name: "UX_UserPermissions_Scope",
                table: "UserPermissionAssignments",
                columns: new[] { "UserId", "PermissionId", "ScopeType", "ResourceId" },
                unique: true,
                filter: "[ResourceId] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RolePermission_Scope",
                table: "RolePermissions",
                sql: "(ScopeType = 0 AND ResourceId IS NULL)\r\n                      OR (ScopeType IN (1,2) AND ResourceId IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_Parent",
                table: "Resources",
                columns: new[] { "ParentResourceType", "ParentReferenceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Resources_ResourceId",
                table: "RolePermissions",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Resources_ResourceId",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "UX_UserPermissions_Scope",
                table: "UserPermissionAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RolePermission_Scope",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_Resources_Parent",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "ParentResourceType",
                table: "Resources");

            migrationBuilder.RenameColumn(
                name: "ParentReferenceId",
                table: "Resources",
                newName: "CommitteeId");

            migrationBuilder.RenameIndex(
                name: "UX_Resources_Type_ExternalRef",
                table: "Resources",
                newName: "UX_Resources_Type_Ref");

            migrationBuilder.AddColumn<Guid>(
                name: "CommitteeId",
                table: "UserPermissionAssignments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ResourceId",
                table: "RolePermissions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "CommitteeId",
                table: "RolePermissions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserPerm_User_Committee",
                table: "UserPermissionAssignments",
                columns: new[] { "UserId", "CommitteeId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserPerm_User_Permission",
                table: "UserPermissionAssignments",
                columns: new[] { "UserId", "PermissionId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserPerm_User_Resource",
                table: "UserPermissionAssignments",
                columns: new[] { "UserId", "ResourceId" });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_Committee",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId", "CommitteeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Resources_Committee",
                table: "Resources",
                column: "CommitteeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Resources_ResourceId",
                table: "RolePermissions",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id");
        }
    }
}
