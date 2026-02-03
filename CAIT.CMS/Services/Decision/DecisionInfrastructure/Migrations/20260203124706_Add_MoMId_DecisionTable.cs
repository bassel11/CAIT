using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DecisionInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_MoMId_DecisionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MoMId",
                table: "Decisions",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoMId",
                table: "Decisions");
        }
    }
}
