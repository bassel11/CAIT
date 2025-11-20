using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommitteeInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeysToStatusHistories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommitteeStatusHistories_CommitteeStatuses_CommitteeStatusId",
                table: "CommitteeStatusHistories");

            migrationBuilder.DropIndex(
                name: "IX_CommitteeStatusHistories_CommitteeStatusId",
                table: "CommitteeStatusHistories");

            migrationBuilder.DropColumn(
                name: "CommitteeStatusId",
                table: "CommitteeStatusHistories");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeStatusHistories_NewStatusId",
                table: "CommitteeStatusHistories",
                column: "NewStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeStatusHistories_OldStatusId",
                table: "CommitteeStatusHistories",
                column: "OldStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommitteeStatusHistories_CommitteeStatuses_NewStatusId",
                table: "CommitteeStatusHistories",
                column: "NewStatusId",
                principalTable: "CommitteeStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommitteeStatusHistories_CommitteeStatuses_OldStatusId",
                table: "CommitteeStatusHistories",
                column: "OldStatusId",
                principalTable: "CommitteeStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommitteeStatusHistories_CommitteeStatuses_NewStatusId",
                table: "CommitteeStatusHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_CommitteeStatusHistories_CommitteeStatuses_OldStatusId",
                table: "CommitteeStatusHistories");

            migrationBuilder.DropIndex(
                name: "IX_CommitteeStatusHistories_NewStatusId",
                table: "CommitteeStatusHistories");

            migrationBuilder.DropIndex(
                name: "IX_CommitteeStatusHistories_OldStatusId",
                table: "CommitteeStatusHistories");

            migrationBuilder.AddColumn<int>(
                name: "CommitteeStatusId",
                table: "CommitteeStatusHistories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeStatusHistories_CommitteeStatusId",
                table: "CommitteeStatusHistories",
                column: "CommitteeStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommitteeStatusHistories_CommitteeStatuses_CommitteeStatusId",
                table: "CommitteeStatusHistories",
                column: "CommitteeStatusId",
                principalTable: "CommitteeStatuses",
                principalColumn: "Id");
        }
    }
}
