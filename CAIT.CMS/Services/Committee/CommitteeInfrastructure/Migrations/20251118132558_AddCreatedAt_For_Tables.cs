using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommitteeInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAt_For_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CommitteeStatusHistories",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CommitteeStatuses",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CommitteeMembers",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CommitteeMemberRoles",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.UpdateData(
                table: "CommitteeStatuses",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 13, 25, 57, 494, DateTimeKind.Utc).AddTicks(2529));

            migrationBuilder.UpdateData(
                table: "CommitteeStatuses",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 13, 25, 57, 494, DateTimeKind.Utc).AddTicks(3526));

            migrationBuilder.UpdateData(
                table: "CommitteeStatuses",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 13, 25, 57, 494, DateTimeKind.Utc).AddTicks(3528));

            migrationBuilder.UpdateData(
                table: "CommitteeStatuses",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 13, 25, 57, 494, DateTimeKind.Utc).AddTicks(3529));

            migrationBuilder.UpdateData(
                table: "CommitteeStatuses",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 13, 25, 57, 494, DateTimeKind.Utc).AddTicks(3530));

            migrationBuilder.UpdateData(
                table: "CommitteeStatuses",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 13, 25, 57, 494, DateTimeKind.Utc).AddTicks(3531));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CommitteeStatusHistories");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CommitteeStatuses");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CommitteeMembers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CommitteeMemberRoles");
        }
    }
}
