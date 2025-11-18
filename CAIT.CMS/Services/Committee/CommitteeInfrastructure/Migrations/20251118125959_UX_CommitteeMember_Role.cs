using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommitteeInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UX_CommitteeMember_Role : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CommitteeMemberRoles_CommitteeMemberId",
                table: "CommitteeMemberRoles");

            migrationBuilder.CreateIndex(
                name: "UX_CommitteeMember_Role",
                table: "CommitteeMemberRoles",
                columns: new[] { "CommitteeMemberId", "RoleId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_CommitteeMember_Role",
                table: "CommitteeMemberRoles");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeMemberRoles_CommitteeMemberId",
                table: "CommitteeMemberRoles",
                column: "CommitteeMemberId");
        }
    }
}
