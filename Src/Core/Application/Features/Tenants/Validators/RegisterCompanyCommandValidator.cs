using Application.Features.Tenants.Commands;
using FluentValidation;

namespace Application.Features.Tenants.Validators
{
    public sealed class RegisterCompanyCommandValidator : AbstractValidator<RegisterCompanyCommand>
    {
        public RegisterCompanyCommandValidator()
        {
            RuleFor(x => x.TenantName)
                .MaximumLength(150)
                .NotEmpty();

            RuleFor(x => x.Identifier)
                .MaximumLength(63)
                .NotEmpty();

            RuleFor(x => x.FirstName)
                .MaximumLength(250)
                .NotEmpty();

            RuleFor(x => x.LastName)
                .MaximumLength(250)
                .NotEmpty();

            RuleFor(x => x.Password)
                .MaximumLength(255)
                .NotEmpty();

            RuleFor(x => x.Email)
                .EmailAddress()
                .NotEmpty();
        }
    }
}
