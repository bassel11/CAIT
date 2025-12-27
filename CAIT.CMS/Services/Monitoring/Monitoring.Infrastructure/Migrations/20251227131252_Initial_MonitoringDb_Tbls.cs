using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Monitoring.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial_MonitoringDb_Tbls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommitteeSummaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastActivityDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MemberCount = table.Column<int>(type: "int", nullable: false),
                    CompletedTasksCount = table.Column<int>(type: "int", nullable: false),
                    OverdueTasksCount = table.Column<int>(type: "int", nullable: false),
                    AttendanceRate = table.Column<double>(type: "float", nullable: false),
                    IsCompliant = table.Column<bool>(type: "bit", nullable: false),
                    NonComplianceReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommitteeSummaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MemberWorkloads",
                columns: table => new
                {
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalCommittees = table.Column<int>(type: "int", nullable: false),
                    PendingTasks = table.Column<int>(type: "int", nullable: false),
                    OverdueTasks = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberWorkloads", x => x.MemberId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeSummaries_LastActivityDate",
                table: "CommitteeSummaries",
                column: "LastActivityDate");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeSummaries_Status",
                table: "CommitteeSummaries",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommitteeSummaries");

            migrationBuilder.DropTable(
                name: "MemberWorkloads");
        }
    }
}
