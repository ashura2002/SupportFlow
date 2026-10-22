using Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Domain.Entities
{
    public class Tenant : BaseEntity
    {
        public string Name { get; private set; }
        public string Identifier { get; private set; }
        public string SchemaName { get; private set; }
        public bool IsActive { get; private set; }



        private Tenant(
            string name,
            string identifier,
            string schemaName)
        {
            Name = name;
            Identifier = identifier;
            SchemaName = schemaName;
            IsActive = true;
        }

        public static Tenant Create(
            string name,
            string identifier,
            string schemaName)
        {
            name = ValidateName(name);
            identifier = ValidateIdentifier(identifier);
            schemaName = ValidateSchemaName(schemaName);

            return new Tenant(
                name,
                identifier,
                schemaName);
        }


        public void UpdateTenantName(string newName)
        {
            if (Name == newName)
                return;

            Name = newName;
            Touch();
        }

        public void Deactivate()
        {
            if (!IsActive)
                return;

            IsActive = false;
            Touch();
        }

        public void Activate()
        {
            if (IsActive)
                return;

            IsActive = true;
            Touch();
        }

        private static string ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainRuleViolationException("Tenant name cannot be empty.");

            name = name.Trim();
            if (name.Length > 150)
                throw new DomainRuleViolationException(
                    "Tenant name cannot exceed 150 characters.");

            return name;
        }

        private static string ValidateIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new DomainRuleViolationException("Identifier cannot be empty.");

            identifier = identifier.Trim().ToLowerInvariant();
            if (identifier.Length > 63 ||
               !Regex.IsMatch(identifier, @"^[a-z0-9]+(?:-[a-z0-9]+)*$"))
                throw new DomainRuleViolationException(
                    "Tenant identifier has an invalid format.");

            return identifier;
        }

        private static string ValidateSchemaName(string schemaName)
        {
            if (string.IsNullOrWhiteSpace(schemaName))
                throw new DomainRuleViolationException(
                    "Tenant schema name cannot be empty.");

            schemaName = schemaName.Trim().ToLowerInvariant();

            if (schemaName.Length > 63 ||
                !Regex.IsMatch(schemaName, @"^[a-z_][a-z0-9_]*$") ||
                // reserved for shared platform data
                schemaName == "public" ||
                // reserved prefix for PostgreSQL system schemas
                schemaName.StartsWith("pg_", StringComparison.Ordinal))
                throw new DomainRuleViolationException(
                    "Tenant schema name is invalid or reserved.");

            return schemaName;
        }

    }
}
