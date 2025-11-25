using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommitteeInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCommittee_Quorum_Rule_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CommitteeMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CommitteeQuorumRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommitteeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ThresholdPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AbsoluteCount = table.Column<int>(type: "int", nullable: true),
                    UsePlusOne = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommitteeQuorumRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommitteeQuorumRules_Committees_CommitteeId",
                        column: x => x.CommitteeId,
                        principalTable: "Committees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeQuorumRules_CommitteeId",
                table: "CommitteeQuorumRules",
                column: "CommitteeId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommitteeQuorumRules");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CommitteeMembers");
        }
    }
}
