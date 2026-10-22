using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantIdPropertyInTicketAuditEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "TicketAuditEntries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TicketAuditEntries_TenantId",
                table: "TicketAuditEntries",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketAuditEntries_Tenants_TenantId",
                table: "TicketAuditEntries",
                column: "TenantId",
                principalSchema: "public",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketAuditEntries_Tenants_TenantId",
                table: "TicketAuditEntries");

            migrationBuilder.DropIndex(
                name: "IX_TicketAuditEntries_TenantId",
                table: "TicketAuditEntries");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TicketAuditEntries");
        }
    }
}
