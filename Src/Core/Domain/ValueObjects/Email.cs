using Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects
{
    public sealed record Email
    {
        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainRuleViolationException("Email cannot be empty.");

            value = value.Trim();
            if (!IsValidEmail(value))
                throw new DomainRuleViolationException("Email invalid format.");

            return new Email(value);

        }

        private static bool IsValidEmail(string email)
        {
            var pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

    }
}
