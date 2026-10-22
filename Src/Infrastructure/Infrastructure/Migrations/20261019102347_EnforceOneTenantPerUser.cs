using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnforceOneTenantPerUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TenantMemberships_UserId_TenantId",
                schema: "public",
                table: "TenantMemberships");

            migrationBuilder.CreateIndex(
                name: "IX_TenantMemberships_UserId",
                schema: "public",
                table: "TenantMemberships",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TenantMemberships_UserId",
                schema: "public",
                table: "TenantMemberships");

            migrationBuilder.CreateIndex(
                name: "IX_TenantMemberships_UserId_TenantId",
                schema: "public",
                table: "TenantMemberships",
                columns: new[] { "UserId", "TenantId" },
                unique: true);
        }
    }
}
