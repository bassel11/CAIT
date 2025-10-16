using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Email_authentication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MFASecret",
                table: "Users",
                newName: "MFACode");

            migrationBuilder.AddColumn<DateTime>(
                name: "MFACodeExpiry",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MFACodeExpiry",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "MFACode",
                table: "Users",
                newName: "MFASecret");
        }
    }
}
