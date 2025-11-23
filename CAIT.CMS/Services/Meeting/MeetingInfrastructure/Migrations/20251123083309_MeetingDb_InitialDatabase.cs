using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MeetingDb_InitialDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Meetings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommitteeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    RecurrenceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RecurrenceRule = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TeamsLink = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OutlookEventId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meetings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgendaItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MeetingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgendaItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgendaItems_Meetings_MeetingId",
                        column: x => x.MeetingId,
                        principalTable: "Meetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AIGeneratedContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MeetingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Prompt = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    GeneratedText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIGeneratedContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIGeneratedContents_Meetings_MeetingId",
                        column: x => x.MeetingId,
                        principalTable: "Meetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MeetingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RSVP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AttendanceStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceRecords_Meetings_MeetingId",
                        column: x => x.MeetingId,
                        principalTable: "Meetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeetingDecisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MeetingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GovernanceModel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RelatedAgendaItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingDecisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeetingDecisions_Meetings_MeetingId",
                        column: x => x.MeetingId,
                        principalTable: "Meetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeetingIntegrationLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MeetingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IntegrationType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingIntegrationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeetingIntegrationLogs_Meetings_MeetingId",
                        column: x => x.MeetingId,
                        principalTable: "Meetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeetingNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MeetingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Processed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeetingNotifications_Meetings_MeetingId",
                        column: x => x.MeetingId,
                        principalTable: "Meetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MinutesOfMeetings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MeetingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AttendanceSummary = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    AgendaSummary = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    DecisionsSummary = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ActionItemsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinutesOfMeetings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MinutesOfMeetings_Meetings_MeetingId",
                        column: x => x.MeetingId,
                        principalTable: "Meetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeetingVotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DecisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Choice = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeetingVotes_MeetingDecisions_DecisionId",
                        column: x => x.DecisionId,
                        principalTable: "MeetingDecisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MinutesVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MoMId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinutesVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MinutesVersions_MinutesOfMeetings_MoMId",
                        column: x => x.MoMId,
                        principalTable: "MinutesOfMeetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgendaItems_MeetingId",
                table: "AgendaItems",
                column: "MeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_AgendaItems_MeetingId_SortOrder",
                table: "AgendaItems",
                columns: new[] { "MeetingId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_AIGeneratedContents_ContentType",
                table: "AIGeneratedContents",
                column: "ContentType");

            migrationBuilder.CreateIndex(
                name: "IX_AIGeneratedContents_MeetingId",
                table: "AIGeneratedContents",
                column: "MeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_MeetingId",
                table: "AttendanceRecords",
                column: "MeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_MeetingId_MemberId",
                table: "AttendanceRecords",
                columns: new[] { "MeetingId", "MemberId" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_MemberId",
                table: "AttendanceRecords",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingDecisions_MeetingId",
                table: "MeetingDecisions",
                column: "MeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingIntegrationLogs_IntegrationType",
                table: "MeetingIntegrationLogs",
                column: "IntegrationType");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingIntegrationLogs_MeetingId",
                table: "MeetingIntegrationLogs",
                column: "MeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingNotifications_MeetingId",
                table: "MeetingNotifications",
                column: "MeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingNotifications_Processed_CreatedAt",
                table: "MeetingNotifications",
                columns: new[] { "Processed", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_CommitteeId",
                table: "Meetings",
                column: "CommitteeId");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_OutlookEventId",
                table: "Meetings",
                column: "OutlookEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_StartDate",
                table: "Meetings",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_Status",
                table: "Meetings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingVotes_DecisionId",
                table: "MeetingVotes",
                column: "DecisionId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingVotes_DecisionId_MemberId",
                table: "MeetingVotes",
                columns: new[] { "DecisionId", "MemberId" });

            migrationBuilder.CreateIndex(
                name: "IX_MinutesOfMeetings_MeetingId",
                table: "MinutesOfMeetings",
                column: "MeetingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MinutesOfMeetings_MeetingId_VersionNumber",
                table: "MinutesOfMeetings",
                columns: new[] { "MeetingId", "VersionNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_MinutesVersions_MoMId",
                table: "MinutesVersions",
                column: "MoMId");

            migrationBuilder.CreateIndex(
                name: "IX_MinutesVersions_MoMId_VersionNumber",
                table: "MinutesVersions",
                columns: new[] { "MoMId", "VersionNumber" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgendaItems");

            migrationBuilder.DropTable(
                name: "AIGeneratedContents");

            migrationBuilder.DropTable(
                name: "AttendanceRecords");

            migrationBuilder.DropTable(
                name: "MeetingIntegrationLogs");

            migrationBuilder.DropTable(
                name: "MeetingNotifications");

            migrationBuilder.DropTable(
                name: "MeetingVotes");

            migrationBuilder.DropTable(
                name: "MinutesVersions");

            migrationBuilder.DropTable(
                name: "MeetingDecisions");

            migrationBuilder.DropTable(
                name: "MinutesOfMeetings");

            migrationBuilder.DropTable(
                name: "Meetings");
        }
    }
}
