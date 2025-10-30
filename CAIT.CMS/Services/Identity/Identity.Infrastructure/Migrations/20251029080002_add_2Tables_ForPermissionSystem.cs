using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_2Tables_ForPermissionSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Resource",
                table: "Permissions",
                newName: "ResourceType");

            migrationBuilder.AddColumn<bool>(
                name: "Allow",
                table: "RolePermissions",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CommitteeId",
                table: "RolePermissions",
                type: "uniqueidentifier",
                nullable: true);

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
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResourceType = table.Column<int>(type: "int", nullable: false),
                    CommitteeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissionAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScopeType = table.Column<int>(type: "int", nullable: false),
                    CommitteeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Allow = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResourceId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissionAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPermissionAssignments_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermissionAssignments_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPermissionAssignments_Resources_ResourceId1",
                        column: x => x.ResourceId1,
                        principalTable: "Resources",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserPermissionAssignments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_Committee",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId", "CommitteeId" });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_ResourceId",
                table: "RolePermissions",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_Committee",
                table: "Resources",
                column: "CommitteeId");

            migrationBuilder.CreateIndex(
                name: "UX_Resources_Type_Ref",
                table: "Resources",
                columns: new[] { "ResourceType", "ReferenceId" },
                unique: true);

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
                name: "IX_UserPermissionAssignments_PermissionId",
                table: "UserPermissionAssignments",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionAssignments_ResourceId",
                table: "UserPermissionAssignments",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionAssignments_ResourceId1",
                table: "UserPermissionAssignments",
                column: "ResourceId1");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Resources_ResourceId",
                table: "RolePermissions",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Resources_ResourceId",
                table: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserPermissionAssignments");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_Committee",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_ResourceId",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "Allow",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "CommitteeId",
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

            migrationBuilder.RenameColumn(
                name: "ResourceType",
                table: "Permissions",
                newName: "Resource");
        }
    }
}
