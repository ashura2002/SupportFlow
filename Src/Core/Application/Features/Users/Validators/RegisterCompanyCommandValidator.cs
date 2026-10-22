using Application.Features.Tenants.Commands;
using FluentValidation;

namespace Application.Features.Users.Validators
{
    public sealed class RegisterCompanyCommandValidator
        : AbstractValidator<RegisterCompanyCommand>
    {
        public RegisterCompanyCommandValidator()
        {
            RuleFor(x => x.TenantName)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.Identifier)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(50)
                .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
                .WithMessage(
                    "Identifier must contain lowercase letters, numbers, and single hyphens only.");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(128);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(254);
        }
    }
}
