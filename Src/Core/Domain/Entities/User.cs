using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Password { get; private set; }
        public Email Email { get; private set; }
        public Roles Role { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public string FullName => $"{FirstName} {LastName}";

        private User(
            string firstName,
            string lastName,
            string password,
            Email email,
            Roles role)
        {
            FirstName = firstName;
            LastName = lastName;
            Password = password;
            Email = email;
            Role = role;
        }


        public static User Create(
            string firstName,
            string lastName,
            string password,
            Email email,
            Roles role)
        {
            firstName = ValidateFirstName(firstName);
            lastName = ValidateLastName(lastName);
            password = ValidatePassword(password);
            return new User(firstName, lastName, password, email, role);
        }


        public void UpdateFirstName(string newFirstName)
        {
            EnsureNotDeleted("Cannot update first name for deleted user.");
            newFirstName = ValidateFirstName(newFirstName);
            if (FirstName == newFirstName)
                return;

            FirstName = newFirstName;
            Touch();
        }

        public void UpdateLastName(string newLastName)
        {
            EnsureNotDeleted("Cannot update last name for deleted user.");
            newLastName = ValidateLastName(newLastName);
            if (LastName == newLastName)
                return;

            LastName = newLastName;
            Touch();
        }

        public void UpdatePassword(string newPassword)
        {
            EnsureNotDeleted("Cannot update password for deleted user.");
            newPassword = ValidatePassword(newPassword);
            if (Password == newPassword)
                return;

            Password = newPassword;
            Touch();
        }

        public void SoftDelete()
        {
            if (Role == Roles.Administrator)
                throw new DomainRuleViolationException("Admin account cannot be deleted.");

            if (DeletedAt.HasValue)
                return;

            DeletedAt = DateTime.UtcNow;
            Touch();
        }


        private static string ValidateFirstName(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainRuleViolationException("First name cannot be empty.");

            firstName = firstName.Trim();
            if (firstName.Length < 3)
                throw new DomainRuleViolationException("First name must be at least 3 characters long.");

            return firstName;
        }

        private static string ValidateLastName(string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainRuleViolationException("Last name cannot be empty.");

            lastName = lastName.Trim();
            if (lastName.Length < 3)
                throw new DomainRuleViolationException("Last name must be at least 3 characters long.");

            return lastName;
        }

        private static string ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new DomainRuleViolationException("Password cannot be empty.");

            if (password.Length < 8)
                throw new DomainRuleViolationException("Password must be at least 8 characters long.");
            return password;
        }

        private void EnsureNotDeleted(string msg)
        {
            if (DeletedAt.HasValue)
                throw new DomainRuleViolationException(msg);
        }
    }
}
