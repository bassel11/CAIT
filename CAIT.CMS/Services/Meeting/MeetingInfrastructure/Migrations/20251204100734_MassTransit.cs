using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MassTransit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxMessages",
                table: "OutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_Processed_Attempts_LockedBy_OccurredAt",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "Attempts",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "LastError",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "LockedBy",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "Processed",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "OutboxMessages");

            migrationBuilder.RenameColumn(
                name: "ProcessedAt",
                table: "OutboxMessages",
                newName: "ExpirationTime");

            migrationBuilder.RenameColumn(
                name: "Payload",
                table: "OutboxMessages",
                newName: "MessageType");

            migrationBuilder.RenameColumn(
                name: "OccurredAt",
                table: "OutboxMessages",
                newName: "SentTime");

            migrationBuilder.RenameColumn(
                name: "LockedAt",
                table: "OutboxMessages",
                newName: "EnqueueTime");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OutboxMessages",
                newName: "MessageId");

            migrationBuilder.AddColumn<long>(
                name: "SequenceNumber",
                table: "OutboxMessages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "OutboxMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "OutboxMessages",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ConversationId",
                table: "OutboxMessages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CorrelationId",
                table: "OutboxMessages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinationAddress",
                table: "OutboxMessages",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FaultAddress",
                table: "OutboxMessages",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Headers",
                table: "OutboxMessages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InboxConsumerId",
                table: "OutboxMessages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InboxMessageId",
                table: "OutboxMessages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InitiatorId",
                table: "OutboxMessages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OutboxId",
                table: "OutboxMessages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Properties",
                table: "OutboxMessages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RequestId",
                table: "OutboxMessages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponseAddress",
                table: "OutboxMessages",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceAddress",
                table: "OutboxMessages",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxMessages",
                table: "OutboxMessages",
                column: "SequenceNumber");

            migrationBuilder.CreateTable(
                name: "InboxState",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsumerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Received = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceiveCount = table.Column<int>(type: "int", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Consumed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Delivered = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxState", x => x.Id);
                    table.UniqueConstraint("AK_InboxState_MessageId_ConsumerId", x => new { x.MessageId, x.ConsumerId });
                });

            migrationBuilder.CreateTable(
                name: "OutboxStates",
                columns: table => new
                {
                    OutboxId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Delivered = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxStates", x => x.OutboxId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_EnqueueTime",
                table: "OutboxMessages",
                column: "EnqueueTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ExpirationTime",
                table: "OutboxMessages",
                column: "ExpirationTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_InboxMessageId_InboxConsumerId_SequenceNumber",
                table: "OutboxMessages",
                columns: new[] { "InboxMessageId", "InboxConsumerId", "SequenceNumber" },
                unique: true,
                filter: "[InboxMessageId] IS NOT NULL AND [InboxConsumerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_OutboxId_SequenceNumber",
                table: "OutboxMessages",
                columns: new[] { "OutboxId", "SequenceNumber" },
                unique: true,
                filter: "[OutboxId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InboxState_Delivered",
                table: "InboxState",
                column: "Delivered");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxStates_Created",
                table: "OutboxStates",
                column: "Created");

            migrationBuilder.AddForeignKey(
                name: "FK_OutboxMessages_InboxState_InboxMessageId_InboxConsumerId",
                table: "OutboxMessages",
                columns: new[] { "InboxMessageId", "InboxConsumerId" },
                principalTable: "InboxState",
                principalColumns: new[] { "MessageId", "ConsumerId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OutboxMessages_OutboxStates_OutboxId",
                table: "OutboxMessages",
                column: "OutboxId",
                principalTable: "OutboxStates",
                principalColumn: "OutboxId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutboxMessages_InboxState_InboxMessageId_InboxConsumerId",
                table: "OutboxMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_OutboxMessages_OutboxStates_OutboxId",
                table: "OutboxMessages");

            migrationBuilder.DropTable(
                name: "InboxState");

            migrationBuilder.DropTable(
                name: "OutboxStates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxMessages",
                table: "OutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_EnqueueTime",
                table: "OutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_ExpirationTime",
                table: "OutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_InboxMessageId_InboxConsumerId_SequenceNumber",
                table: "OutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_OutboxId_SequenceNumber",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "SequenceNumber",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "Body",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "ConversationId",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "DestinationAddress",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "FaultAddress",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "Headers",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "InboxConsumerId",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "InboxMessageId",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "InitiatorId",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "OutboxId",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "Properties",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "ResponseAddress",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "SourceAddress",
                table: "OutboxMessages");

            migrationBuilder.RenameColumn(
                name: "SentTime",
                table: "OutboxMessages",
                newName: "OccurredAt");

            migrationBuilder.RenameColumn(
                name: "MessageType",
                table: "OutboxMessages",
                newName: "Payload");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "OutboxMessages",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ExpirationTime",
                table: "OutboxMessages",
                newName: "ProcessedAt");

            migrationBuilder.RenameColumn(
                name: "EnqueueTime",
                table: "OutboxMessages",
                newName: "LockedAt");

            migrationBuilder.AddColumn<int>(
                name: "Attempts",
                table: "OutboxMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LastError",
                table: "OutboxMessages",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LockedBy",
                table: "OutboxMessages",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Processed",
                table: "OutboxMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "OutboxMessages",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "OutboxMessages",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxMessages",
                table: "OutboxMessages",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Processed_Attempts_LockedBy_OccurredAt",
                table: "OutboxMessages",
                columns: new[] { "Processed", "Attempts", "LockedBy", "OccurredAt" });
        }
    }
}
