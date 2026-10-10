using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public sealed class SupportFlowDbContext(DbContextOptions<SupportFlowDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<TicketReply> TicketReplies => Set<TicketReply>();
        public DbSet<TicketAttachmentReply> TicketAttachmentReply => Set<TicketAttachmentReply>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SupportFlowDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
