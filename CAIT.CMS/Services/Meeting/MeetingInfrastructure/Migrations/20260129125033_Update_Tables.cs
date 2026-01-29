using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuorumAbsoluteCount",
                table: "Meetings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "QuorumThresholdPercent",
                table: "Meetings",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuorumType",
                table: "Meetings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "QuorumUsePlusOne",
                table: "Meetings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsProxy",
                table: "Attendances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Attendances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProxyName",
                table: "Attendances",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuorumAbsoluteCount",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "QuorumThresholdPercent",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "QuorumType",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "QuorumUsePlusOne",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "IsProxy",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "ProxyName",
                table: "Attendances");
        }
    }
}
