using Application.Interfaces.Services;
using Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public sealed class TenantDbContext : DbContext
    {
        // para sa tenant specific schema
        private readonly ITenantContext _tenantContext;
        public TenantDbContext(DbContextOptions<TenantDbContext> options, ITenantContext tenantContext) : base(options)
        {
            _tenantContext = tenantContext;
        }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<TicketReply> TicketReplies => Set<TicketReply>();
        public DbSet<TicketAttachmentReply> TicketAttachmentReply => Set<TicketAttachmentReply>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<TicketAuditEntry> TicketAuditEntries => Set<TicketAuditEntry>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(_tenantContext.SchemaName);

            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new TicketConfiguration());
            modelBuilder.ApplyConfiguration(new TicketReplyConfiguration());
            modelBuilder.ApplyConfiguration(new TicketAttachmentReplyConfiguration());
            modelBuilder.ApplyConfiguration(new TicketAuditEntryConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
