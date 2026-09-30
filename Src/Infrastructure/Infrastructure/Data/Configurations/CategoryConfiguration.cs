using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(category => category.Id);

            builder.Property(category => category.Name)
                .IsRequired();

            builder.Property(category => category.Description)
                .IsRequired(false);

            builder.HasIndex(category => category.Name).IsUnique();
            builder.HasQueryFilter(category => category.DeletedAt == null);
        }
    }
}
