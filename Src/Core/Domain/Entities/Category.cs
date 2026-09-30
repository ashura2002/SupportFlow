using Domain.Exceptions;

namespace Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public DateTime? DeletedAt { get; private set; }

        private Category(string name, string? description)
        {
            Name = name;
            Description = description;
        }

        public static Category Create(string name, string? description)
        {
            name = ValidateCategoryName(name);
            description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            return new Category(name, description);
        }

        public void SoftDelete()
        {
            if (DeletedAt.HasValue)
                return;

            DeletedAt = DateTime.UtcNow;
            Touch();
        }

        public void UpdateCategoryName(string value)
        {
            value = ValidateCategoryName(value);
            if (Name == value)
                return;

            Name = value;
            Touch();
        }

        public void UpdateDescription(string? value)
        {
            value = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
            if (Description == value)
                return;

            Description = value;
            Touch();
        }

        private static string ValidateCategoryName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainRuleViolationException("Category name cannot be empty.");

            value = value.Trim();
            if (value.Length < 3)
                throw new DomainRuleViolationException("Category name must be at least 3 characters long.");
            return value;
        }
    }
}
