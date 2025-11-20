using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommitteeInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class createNewIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CommitteeStatusHistories_CommitteeId_ChangedAt",
                table: "CommitteeStatusHistories",
                columns: new[] { "CommitteeId", "ChangedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CommitteeStatusHistories_CommitteeId_ChangedAt",
                table: "CommitteeStatusHistories");
        }
    }
}
