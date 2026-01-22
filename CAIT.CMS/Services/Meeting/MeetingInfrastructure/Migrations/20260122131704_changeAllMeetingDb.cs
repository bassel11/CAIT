using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeAllMeetingDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_Meetings_MeetingId",
                table: "AttendanceRecords");

            migrationBuilder.DropTable(
                name: "MeetingIntegrationLogs");

            migrationBuilder.DropTable(
                name: "MeetingNotifications");

            migrationBuilder.DropTable(
                name: "MeetingVotes");

            migrationBuilder.DropTable(
                name: "MeetingDecisions");

            migrationBuilder.DropIndex(
                name: "IX_MinutesVersions_MoMId",
                table: "MinutesVersions");

            migrationBuilder.DropIndex(
                name: "IX_MinutesVersions_MoMId_VersionNumber",
                table: "MinutesVersions");

            migrationBuilder.DropIndex(
                name: "IX_MinutesOfMeetings_MeetingId_VersionNumber",
                table: "MinutesOfMeetings");

            migrationBuilder.DropIndex(
                name: "IX_Meetings_CommitteeId",
                table: "Meetings");

            migrationBuilder.DropIndex(
                name: "IX_Meetings_OutlookEventId",
                table: "Meetings");

            migrationBuilder.DropIndex(
                name: "IX_Meetings_StartDate",
                table: "Meetings");

            migrationBuilder.DropIndex(
                name: "IX_Meetings_Status",
                table: "Meetings");

            migrationBuilder.DropIndex(
                name: "IX_AgendaItems_MeetingId_SortOrder",
                table: "AgendaItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttendanceRecords",
                table: "AttendanceRecords");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_MeetingId",
                table: "AttendanceRecords");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_MeetingId_MemberId",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "AgendaSummary",
                table: "MinutesOfMeetings");

            migrationBuilder.DropColumn(
                name: "AttendanceSummary",
                table: "MinutesOfMeetings");

            migrationBuilder.DropColumn(
                name: "DecisionsSummary",
                table: "MinutesOfMeetings");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MinutesOfMeetings");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Meetings");

            migrationBuilder.RenameTable(
                name: "AttendanceRecords",
                newName: "Attendances");

            migrationBuilder.RenameColumn(
                name: "ActionItemsJson",
                table: "MinutesOfMeetings",
                newName: "FullContentHtml");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Meetings",
                newName: "LastTimeModified");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "Attendances",
                newName: "LastTimeModified");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "Attendances",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecords_MemberId",
                table: "Attendances",
                newName: "IX_Attendances_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "StoragePath",
                table: "MoMAttachments",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "MoMAttachments",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "MoMAttachments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "MoMAttachments",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "MoMAttachments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "MoMAttachments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTimeModified",
                table: "MoMAttachments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SizeInBytes",
                table: "MoMAttachments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "MinutesVersions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "MinutesVersions",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "MinutesVersions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTimeModified",
                table: "MinutesVersions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "MinutesVersions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "MinutesVersions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "MinutesOfMeetings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "MinutesOfMeetings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTimeModified",
                table: "MinutesOfMeetings",
                type: "datetime2",
                nullable: true);

            // ✅ الكود الصحيح: حذف العمود ثم إضافته ليقوم الـ SQL Server بتوليد القيم
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Meetings");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Meetings",
                type: "rowversion",
                rowVersion: true,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "RecurrenceRule",
                table: "Meetings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRecurring",
                table: "Meetings",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Meetings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Meetings",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Meetings",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Meetings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationAddress",
                table: "Meetings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationOnlineUrl",
                table: "Meetings",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationRoomName",
                table: "Meetings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationType",
                table: "Meetings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                table: "Meetings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "AIGeneratedContents",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<bool>(
                name: "IsApplied",
                table: "AIGeneratedContents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "AIGeneratedContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTimeModified",
                table: "AIGeneratedContents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModelUsed",
                table: "AIGeneratedContents",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "AgendaItems",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "AllocatedTime",
                table: "AgendaItems",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AgendaItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "AgendaItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTimeModified",
                table: "AgendaItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PresenterId",
                table: "AgendaItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Attendances",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckInTime",
                table: "Attendances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Attendances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Attendances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Attendances",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VotingRight",
                table: "Attendances",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attendances",
                table: "Attendances",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "MoMActionItemDrafts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MoMId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AssigneeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTimeModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoMActionItemDrafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MoMActionItemDrafts_MinutesOfMeetings_MoMId",
                        column: x => x.MoMId,
                        principalTable: "MinutesOfMeetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MoMDecisionDrafts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MoMId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTimeModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoMDecisionDrafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MoMDecisionDrafts_MinutesOfMeetings_MoMId",
                        column: x => x.MoMId,
                        principalTable: "MinutesOfMeetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MinutesVersions_MoMId_VersionNumber",
                table: "MinutesVersions",
                columns: new[] { "MoMId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MinutesOfMeetings_Status",
                table: "MinutesOfMeetings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AIGeneratedContents_ModelUsed",
                table: "AIGeneratedContents",
                column: "ModelUsed");

            migrationBuilder.CreateIndex(
                name: "IX_AgendaItems_MeetingId_SortOrder",
                table: "AgendaItems",
                columns: new[] { "MeetingId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_MeetingId_UserId",
                table: "Attendances",
                columns: new[] { "MeetingId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MoMActionItemDrafts_MoMId",
                table: "MoMActionItemDrafts",
                column: "MoMId");

            migrationBuilder.CreateIndex(
                name: "IX_MoMDecisionDrafts_MoMId",
                table: "MoMDecisionDrafts",
                column: "MoMId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Meetings_MeetingId",
                table: "Attendances",
                column: "MeetingId",
                principalTable: "Meetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Meetings_MeetingId",
                table: "Attendances");

            migrationBuilder.DropTable(
                name: "MoMActionItemDrafts");

            migrationBuilder.DropTable(
                name: "MoMDecisionDrafts");

            migrationBuilder.DropIndex(
                name: "IX_MinutesVersions_MoMId_VersionNumber",
                table: "MinutesVersions");

            migrationBuilder.DropIndex(
                name: "IX_MinutesOfMeetings_Status",
                table: "MinutesOfMeetings");

            migrationBuilder.DropIndex(
                name: "IX_AIGeneratedContents_ModelUsed",
                table: "AIGeneratedContents");

            migrationBuilder.DropIndex(
                name: "IX_AgendaItems_MeetingId_SortOrder",
                table: "AgendaItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attendances",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_MeetingId_UserId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "MoMAttachments");

            migrationBuilder.DropColumn(
                name: "LastTimeModified",
                table: "MoMAttachments");

            migrationBuilder.DropColumn(
                name: "SizeInBytes",
                table: "MoMAttachments");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "MinutesVersions");

            migrationBuilder.DropColumn(
                name: "LastTimeModified",
                table: "MinutesVersions");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "MinutesVersions");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "MinutesVersions");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "MinutesOfMeetings");

            migrationBuilder.DropColumn(
                name: "LastTimeModified",
                table: "MinutesOfMeetings");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "LocationAddress",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "LocationOnlineUrl",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "LocationRoomName",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "LocationType",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "TimeZone",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "IsApplied",
                table: "AIGeneratedContents");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "AIGeneratedContents");

            migrationBuilder.DropColumn(
                name: "LastTimeModified",
                table: "AIGeneratedContents");

            migrationBuilder.DropColumn(
                name: "ModelUsed",
                table: "AIGeneratedContents");

            migrationBuilder.DropColumn(
                name: "AllocatedTime",
                table: "AgendaItems");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AgendaItems");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "AgendaItems");

            migrationBuilder.DropColumn(
                name: "LastTimeModified",
                table: "AgendaItems");

            migrationBuilder.DropColumn(
                name: "PresenterId",
                table: "AgendaItems");

            migrationBuilder.DropColumn(
                name: "CheckInTime",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "VotingRight",
                table: "Attendances");

            migrationBuilder.RenameTable(
                name: "Attendances",
                newName: "AttendanceRecords");

            migrationBuilder.RenameColumn(
                name: "FullContentHtml",
                table: "MinutesOfMeetings",
                newName: "ActionItemsJson");

            migrationBuilder.RenameColumn(
                name: "LastTimeModified",
                table: "Meetings",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AttendanceRecords",
                newName: "MemberId");

            migrationBuilder.RenameColumn(
                name: "LastTimeModified",
                table: "AttendanceRecords",
                newName: "Timestamp");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_UserId",
                table: "AttendanceRecords",
                newName: "IX_AttendanceRecords_MemberId");

            migrationBuilder.AlterColumn<string>(
                name: "StoragePath",
                table: "MoMAttachments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "MoMAttachments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "MoMAttachments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "MoMAttachments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "MoMAttachments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "MinutesVersions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "MinutesVersions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "MinutesOfMeetings",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "AgendaSummary",
                table: "MinutesOfMeetings",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AttendanceSummary",
                table: "MinutesOfMeetings",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DecisionsSummary",
                table: "MinutesOfMeetings",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "MinutesOfMeetings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "Meetings",
                type: "rowversion",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "rowversion",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<string>(
                name: "RecurrenceRule",
                table: "Meetings",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRecurring",
                table: "Meetings",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Meetings",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Meetings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "Meetings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "AIGeneratedContents",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "AgendaItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "AttendanceRecords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttendanceRecords",
                table: "AttendanceRecords",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "MeetingDecisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MeetingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GovernanceModel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RelatedAgendaItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false)
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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ExternalId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IntegrationType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Success = table.Column<bool>(type: "bit", nullable: false)
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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NotificationType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Processed = table.Column<bool>(type: "bit", nullable: false),
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
                name: "MeetingVotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DecisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Choice = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_MinutesVersions_MoMId",
                table: "MinutesVersions",
                column: "MoMId");

            migrationBuilder.CreateIndex(
                name: "IX_MinutesVersions_MoMId_VersionNumber",
                table: "MinutesVersions",
                columns: new[] { "MoMId", "VersionNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_MinutesOfMeetings_MeetingId_VersionNumber",
                table: "MinutesOfMeetings",
                columns: new[] { "MeetingId", "VersionNumber" });

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
                name: "IX_AgendaItems_MeetingId_SortOrder",
                table: "AgendaItems",
                columns: new[] { "MeetingId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_MeetingId",
                table: "AttendanceRecords",
                column: "MeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_MeetingId_MemberId",
                table: "AttendanceRecords",
                columns: new[] { "MeetingId", "MemberId" });

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
                name: "IX_MeetingVotes_DecisionId",
                table: "MeetingVotes",
                column: "DecisionId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingVotes_DecisionId_MemberId",
                table: "MeetingVotes",
                columns: new[] { "DecisionId", "MemberId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_Meetings_MeetingId",
                table: "AttendanceRecords",
                column: "MeetingId",
                principalTable: "Meetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
