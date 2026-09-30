
using Application.Features.Users.Commands;
using FluentValidation;

namespace Application.Features.Users.Validators
{
    public sealed class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
    {
        public UpdatePasswordCommandValidator()
        {
            RuleFor(p => p.Password)
                .MinimumLength(8)
                .NotEmpty();
        }
    }
}
