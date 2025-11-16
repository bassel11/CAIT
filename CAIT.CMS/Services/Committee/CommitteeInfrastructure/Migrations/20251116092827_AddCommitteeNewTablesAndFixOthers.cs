using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CommitteeInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCommitteeNewTablesAndFixOthers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "CommitteeMembers");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "CommitteeMemberRoles");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Committees",
                newName: "StatusId");

            migrationBuilder.AlterColumn<string>(
                name: "Affiliation",
                table: "CommitteeMembers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "CommitteeMemberRoles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "CommitteeAuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommitteeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommitteeMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DecisionText = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommitteeAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommitteeAuditLogs_Committees_CommitteeId",
                        column: x => x.CommitteeId,
                        principalTable: "Committees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommitteeStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommitteeStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommitteeStatusHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommitteeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OldStatusId = table.Column<int>(type: "int", nullable: false),
                    NewStatusId = table.Column<int>(type: "int", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DecisionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DecisionDocumentUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommitteeStatusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommitteeStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommitteeStatusHistories_CommitteeStatuses_CommitteeStatusId",
                        column: x => x.CommitteeStatusId,
                        principalTable: "CommitteeStatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommitteeStatusHistories_Committees_CommitteeId",
                        column: x => x.CommitteeId,
                        principalTable: "Committees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CommitteeStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Draft" },
                    { 2, "Active" },
                    { 3, "Suspended" },
                    { 4, "Completed" },
                    { 5, "Dissolved" },
                    { 6, "Archived" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Committees_StatusId",
                table: "Committees",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeAuditLogs_CommitteeId",
                table: "CommitteeAuditLogs",
                column: "CommitteeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeStatusHistories_CommitteeId",
                table: "CommitteeStatusHistories",
                column: "CommitteeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeStatusHistories_CommitteeStatusId",
                table: "CommitteeStatusHistories",
                column: "CommitteeStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Committees_CommitteeStatuses_StatusId",
                table: "Committees",
                column: "StatusId",
                principalTable: "CommitteeStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Committees_CommitteeStatuses_StatusId",
                table: "Committees");

            migrationBuilder.DropTable(
                name: "CommitteeAuditLogs");

            migrationBuilder.DropTable(
                name: "CommitteeStatusHistories");

            migrationBuilder.DropTable(
                name: "CommitteeStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Committees_StatusId",
                table: "Committees");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "CommitteeMemberRoles");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Committees",
                newName: "Status");

            migrationBuilder.AlterColumn<string>(
                name: "Affiliation",
                table: "CommitteeMembers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "CommitteeMembers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "CommitteeMemberRoles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
