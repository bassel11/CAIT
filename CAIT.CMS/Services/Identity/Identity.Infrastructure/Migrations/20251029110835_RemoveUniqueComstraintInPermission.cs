using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueComstraintInPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Permissions_Resource_Action",
                table: "Permissions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "UX_Permissions_Resource_Action",
                table: "Permissions",
                columns: new[] { "ResourceType", "Action" },
                unique: true);
        }
    }
}
