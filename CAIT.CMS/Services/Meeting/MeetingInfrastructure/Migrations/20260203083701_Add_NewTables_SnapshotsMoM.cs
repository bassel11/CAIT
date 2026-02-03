using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_NewTables_SnapshotsMoM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MoMAttendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MoMId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsPresent = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AbsenceReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTimeModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoMAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MoMAttendances_MinutesOfMeetings_MoMId",
                        column: x => x.MoMId,
                        principalTable: "MinutesOfMeetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MoMDiscussions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MoMId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalAgendaItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TopicTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DiscussionContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTimeModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoMDiscussions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MoMDiscussions_MinutesOfMeetings_MoMId",
                        column: x => x.MoMId,
                        principalTable: "MinutesOfMeetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MoMAttendances_MoMId",
                table: "MoMAttendances",
                column: "MoMId");

            migrationBuilder.CreateIndex(
                name: "IX_MoMDiscussions_MoMId",
                table: "MoMDiscussions",
                column: "MoMId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MoMAttendances");

            migrationBuilder.DropTable(
                name: "MoMDiscussions");
        }
    }
}
