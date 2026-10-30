using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations.Tenant
{
    /// <inheritdoc />
    public partial class InitTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tenant-template");

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "tenant-template",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "tenant-template",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                schema: "tenant-template",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketNumber = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequesterId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAgentId = table.Column<Guid>(type: "uuid", nullable: true),
                    DueAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "tenant-template",
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketAuditEntries",
                schema: "tenant-template",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActorUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    OldValue = table.Column<int>(type: "integer", nullable: true),
                    NewValue = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketAuditEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketAuditEntries_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalSchema: "tenant-template",
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketReplies",
                schema: "tenant-template",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketReplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketReplies_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalSchema: "tenant-template",
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketAttachmentReply",
                schema: "tenant-template",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketReplyId = table.Column<Guid>(type: "uuid", nullable: false),
                    PublicImageUrl = table.Column<string>(type: "text", nullable: false),
                    PublicImageId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketAttachmentReply", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketAttachmentReply_TicketReplies_TicketReplyId",
                        column: x => x.TicketReplyId,
                        principalSchema: "tenant-template",
                        principalTable: "TicketReplies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                schema: "tenant-template",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketAttachmentReply_TicketReplyId",
                schema: "tenant-template",
                table: "TicketAttachmentReply",
                column: "TicketReplyId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketAuditEntries_ActorUserId",
                schema: "tenant-template",
                table: "TicketAuditEntries",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketAuditEntries_TicketId",
                schema: "tenant-template",
                table: "TicketAuditEntries",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketReplies_AuthorId",
                schema: "tenant-template",
                table: "TicketReplies",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketReplies_TicketId",
                schema: "tenant-template",
                table: "TicketReplies",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_AssignedAgentId",
                schema: "tenant-template",
                table: "Tickets",
                column: "AssignedAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CategoryId",
                schema: "tenant-template",
                table: "Tickets",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_RequesterId",
                schema: "tenant-template",
                table: "Tickets",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketNumber",
                schema: "tenant-template",
                table: "Tickets",
                column: "TicketNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "tenant-template");

            migrationBuilder.DropTable(
                name: "TicketAttachmentReply",
                schema: "tenant-template");

            migrationBuilder.DropTable(
                name: "TicketAuditEntries",
                schema: "tenant-template");

            migrationBuilder.DropTable(
                name: "TicketReplies",
                schema: "tenant-template");

            migrationBuilder.DropTable(
                name: "Tickets",
                schema: "tenant-template");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "tenant-template");
        }
    }
}
