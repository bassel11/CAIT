using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixRolePermissionRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RolePermission_Scope",
                table: "RolePermissions");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResourceId",
                table: "RolePermissions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId", "ScopeType" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_RolePermission_Scope",
                table: "RolePermissions",
                sql: "(ScopeType = 0 AND ResourceId IS NULL)\r\n          OR (ScopeType IN (1,2) AND ResourceId IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RolePermission_Scope",
                table: "RolePermissions");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResourceId",
                table: "RolePermissions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId", "ScopeType", "ResourceId" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_RolePermission_Scope",
                table: "RolePermissions",
                sql: "(ScopeType = 0 AND ResourceId IS NULL)\r\n                      OR (ScopeType IN (1,2) AND ResourceId IS NOT NULL)");
        }
    }
}
