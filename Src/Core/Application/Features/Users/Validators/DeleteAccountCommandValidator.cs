using Application.Features.Users.Commands;
using FluentValidation;

namespace Application.Features.Users.Validators;

public sealed class DeleteAccountCommandValidator : AbstractValidator<DeleteAccountCommand>
{
    public DeleteAccountCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

    }
}
