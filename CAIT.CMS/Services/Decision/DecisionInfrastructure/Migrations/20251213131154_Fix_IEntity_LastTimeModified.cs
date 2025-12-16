using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DecisionInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Fix_IEntity_LastTimeModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastModified",
                table: "Votes",
                newName: "LastTimeModified");

            migrationBuilder.RenameColumn(
                name: "LastModified",
                table: "Decisions",
                newName: "LastTimeModified");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastTimeModified",
                table: "Votes",
                newName: "LastModified");

            migrationBuilder.RenameColumn(
                name: "LastTimeModified",
                table: "Decisions",
                newName: "LastModified");
        }
    }
}
