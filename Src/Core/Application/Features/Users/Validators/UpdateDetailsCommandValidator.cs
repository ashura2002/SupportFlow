using Application.Features.Users.Commands;
using FluentValidation;

namespace Application.Features.Users.Validators
{
    public sealed class UpdateDetailsCommandValidator : AbstractValidator<UpdateDetailsCommand>
    {
        public UpdateDetailsCommandValidator()
        {
            RuleFor(u => u.FirstName)
                .MinimumLength(3)
                .MaximumLength(10)
                .NotEmpty();

            RuleFor(u => u.LastName)
             .MinimumLength(3)
             .MaximumLength(10)
             .NotEmpty();
        }
    }
}
