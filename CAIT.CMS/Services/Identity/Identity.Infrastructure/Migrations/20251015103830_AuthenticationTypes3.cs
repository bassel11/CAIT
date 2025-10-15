using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AuthenticationTypes3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_AD",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Entra",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AD",
                table: "Users",
                columns: new[] { "AdDomain", "AdAccount" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Entra",
                table: "Users",
                columns: new[] { "AzureTenantId", "AzureObjectId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_AD",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Entra",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AD",
                table: "Users",
                columns: new[] { "AdDomain", "AdAccount" },
                unique: true,
                filter: "[AdDomain] IS NOT NULL AND [AdAccount] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Entra",
                table: "Users",
                columns: new[] { "AzureTenantId", "AzureObjectId" },
                unique: true,
                filter: "[AzureTenantId] IS NOT NULL AND [AzureObjectId] IS NOT NULL");
        }
    }
}
