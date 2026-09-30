using Application.Features.Auth.Command;
using FluentValidation;

namespace Application.Features.Auth.Validators
{
    public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(u => u.Email)
                .EmailAddress()
                .NotEmpty();

            RuleFor(u => u.Password)
                .NotEmpty();
        }
    }
}
