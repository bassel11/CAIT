using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommitteeInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateCommitteetables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ChangedByUserId",
                table: "CommitteeStatusHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Committees",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "CreationDecisionDocumentUrl",
                table: "Committees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "JoinDate",
                table: "CommitteeMembers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LeaveDate",
                table: "CommitteeMembers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "CommitteeMemberRoles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CommitteeMemberRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "CommitteeMemberRoles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "CommitteeMemberRoles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangedByUserId",
                table: "CommitteeStatusHistories");

            migrationBuilder.DropColumn(
                name: "CreationDecisionDocumentUrl",
                table: "Committees");

            migrationBuilder.DropColumn(
                name: "JoinDate",
                table: "CommitteeMembers");

            migrationBuilder.DropColumn(
                name: "LeaveDate",
                table: "CommitteeMembers");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "CommitteeMemberRoles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CommitteeMemberRoles");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "CommitteeMemberRoles");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "CommitteeMemberRoles");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Committees",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
