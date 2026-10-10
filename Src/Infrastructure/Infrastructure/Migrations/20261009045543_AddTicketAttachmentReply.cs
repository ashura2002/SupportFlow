using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketAttachmentReply : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TicketAttachmentReply",
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
                        principalTable: "TicketReplies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketAttachmentReply_TicketReplyId",
                table: "TicketAttachmentReply",
                column: "TicketReplyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketAttachmentReply");
        }
    }
}
