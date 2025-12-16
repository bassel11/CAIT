using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DecisionInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Fix_Vote_DecisionId_Relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Decisions_DecisionId1",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_DecisionId1",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "DecisionId1",
                table: "Votes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DecisionId1",
                table: "Votes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_DecisionId1",
                table: "Votes",
                column: "DecisionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Decisions_DecisionId1",
                table: "Votes",
                column: "DecisionId1",
                principalTable: "Decisions",
                principalColumn: "Id");
        }
    }
}
