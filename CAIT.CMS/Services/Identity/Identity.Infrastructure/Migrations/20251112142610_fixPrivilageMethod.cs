using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixPrivilageMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Resources_ResourceId",
                table: "RolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionAssignments_Resources_ResourceId",
                table: "UserPermissionAssignments");

            migrationBuilder.DropTable(
                name: "PreRolePermissions");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropIndex(
                name: "IX_UserPermissionAssignments_ResourceId",
                table: "UserPermissionAssignments");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_ResourceId",
                table: "RolePermissions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RolePermission_Scope",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "Allow",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "ResourceId",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "ScopeType",
                table: "RolePermissions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Allow",
                table: "RolePermissions",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "RolePermissions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ResourceId",
                table: "RolePermissions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScopeType",
                table: "RolePermissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PreRolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreRolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_PreRolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PreRolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ParentResourceType = table.Column<int>(type: "int", maxLength: 100, nullable: true),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResourceType = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionAssignments_ResourceId",
                table: "UserPermissionAssignments",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_ResourceId",
                table: "RolePermissions",
                column: "ResourceId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RolePermission_Scope",
                table: "RolePermissions",
                sql: "(ScopeType = 0 AND ResourceId IS NULL)\r\n          OR (ScopeType IN (1,2) AND ResourceId IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_PreRolePermissions_PermissionId",
                table: "PreRolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_Parent",
                table: "Resources",
                columns: new[] { "ParentResourceType", "ParentReferenceId" });

            migrationBuilder.CreateIndex(
                name: "UX_Resources_Type_ExternalRef",
                table: "Resources",
                columns: new[] { "ResourceType", "ReferenceId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Resources_ResourceId",
                table: "RolePermissions",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionAssignments_Resources_ResourceId",
                table: "UserPermissionAssignments",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
