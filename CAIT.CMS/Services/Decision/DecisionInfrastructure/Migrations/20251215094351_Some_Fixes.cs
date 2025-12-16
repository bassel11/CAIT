using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DecisionInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Some_Fixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutboxMessage_InboxState_InboxMessageId_InboxConsumerId",
                table: "OutboxMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_OutboxMessage_OutboxState_OutboxId",
                table: "OutboxMessage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxState",
                table: "OutboxState");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxMessage",
                table: "OutboxMessage");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_InboxState_MessageId_ConsumerId",
                table: "InboxState");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InboxState",
                table: "InboxState");

            migrationBuilder.RenameTable(
                name: "OutboxState",
                newName: "OutboxStates");

            migrationBuilder.RenameTable(
                name: "OutboxMessage",
                newName: "OutboxMessages");

            migrationBuilder.RenameTable(
                name: "InboxState",
                newName: "InboxStates");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxState_Created",
                table: "OutboxStates",
                newName: "IX_OutboxStates_Created");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxMessage_OutboxId_SequenceNumber",
                table: "OutboxMessages",
                newName: "IX_OutboxMessages_OutboxId_SequenceNumber");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxMessage_InboxMessageId_InboxConsumerId_SequenceNumber",
                table: "OutboxMessages",
                newName: "IX_OutboxMessages_InboxMessageId_InboxConsumerId_SequenceNumber");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxMessage_ExpirationTime",
                table: "OutboxMessages",
                newName: "IX_OutboxMessages_ExpirationTime");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxMessage_EnqueueTime",
                table: "OutboxMessages",
                newName: "IX_OutboxMessages_EnqueueTime");

            migrationBuilder.RenameIndex(
                name: "IX_InboxState_Delivered",
                table: "InboxStates",
                newName: "IX_InboxStates_Delivered");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxStates",
                table: "OutboxStates",
                column: "OutboxId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxMessages",
                table: "OutboxMessages",
                column: "SequenceNumber");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_InboxStates_MessageId_ConsumerId",
                table: "InboxStates",
                columns: new[] { "MessageId", "ConsumerId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_InboxStates",
                table: "InboxStates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OutboxMessages_InboxStates_InboxMessageId_InboxConsumerId",
                table: "OutboxMessages",
                columns: new[] { "InboxMessageId", "InboxConsumerId" },
                principalTable: "InboxStates",
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
                name: "FK_OutboxMessages_InboxStates_InboxMessageId_InboxConsumerId",
                table: "OutboxMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_OutboxMessages_OutboxStates_OutboxId",
                table: "OutboxMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxStates",
                table: "OutboxStates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxMessages",
                table: "OutboxMessages");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_InboxStates_MessageId_ConsumerId",
                table: "InboxStates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InboxStates",
                table: "InboxStates");

            migrationBuilder.RenameTable(
                name: "OutboxStates",
                newName: "OutboxState");

            migrationBuilder.RenameTable(
                name: "OutboxMessages",
                newName: "OutboxMessage");

            migrationBuilder.RenameTable(
                name: "InboxStates",
                newName: "InboxState");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxStates_Created",
                table: "OutboxState",
                newName: "IX_OutboxState_Created");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxMessages_OutboxId_SequenceNumber",
                table: "OutboxMessage",
                newName: "IX_OutboxMessage_OutboxId_SequenceNumber");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxMessages_InboxMessageId_InboxConsumerId_SequenceNumber",
                table: "OutboxMessage",
                newName: "IX_OutboxMessage_InboxMessageId_InboxConsumerId_SequenceNumber");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxMessages_ExpirationTime",
                table: "OutboxMessage",
                newName: "IX_OutboxMessage_ExpirationTime");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxMessages_EnqueueTime",
                table: "OutboxMessage",
                newName: "IX_OutboxMessage_EnqueueTime");

            migrationBuilder.RenameIndex(
                name: "IX_InboxStates_Delivered",
                table: "InboxState",
                newName: "IX_InboxState_Delivered");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxState",
                table: "OutboxState",
                column: "OutboxId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxMessage",
                table: "OutboxMessage",
                column: "SequenceNumber");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_InboxState_MessageId_ConsumerId",
                table: "InboxState",
                columns: new[] { "MessageId", "ConsumerId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_InboxState",
                table: "InboxState",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OutboxMessage_InboxState_InboxMessageId_InboxConsumerId",
                table: "OutboxMessage",
                columns: new[] { "InboxMessageId", "InboxConsumerId" },
                principalTable: "InboxState",
                principalColumns: new[] { "MessageId", "ConsumerId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OutboxMessage_OutboxState_OutboxId",
                table: "OutboxMessage",
                column: "OutboxId",
                principalTable: "OutboxState",
                principalColumn: "OutboxId");
        }
    }
}
