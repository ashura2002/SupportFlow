using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Stores shared user accounts in the public schema
            builder.ToTable("Users", "public");

            builder.HasKey(user => user.Id);

            builder.Property(user => user.FirstName)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(user => user.LastName)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(user => user.Password)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(user => user.Email)
                .HasConversion(user => user.Value, user => Email.Create(user))
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(user => user.PlatformRole)
                .HasConversion<int>()
                .IsRequired()
                .HasDefaultValue(PlatformRole.NormalUser);

            builder.HasIndex(user => user.Email)
                .IsUnique();

            builder.HasQueryFilter(user => user.DeletedAt == null);
        }
    }
}
