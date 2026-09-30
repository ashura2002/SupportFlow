using Application.Features.Users.Commands;
using FluentValidation;

namespace Application.Features.Users.Validators
{
    public sealed class CreateRequesterCommandValidator : AbstractValidator<CreateRequesterCommand>
    {
        public CreateRequesterCommandValidator()
        {
            RuleFor(user => user.FirstName)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(10);

            RuleFor(user => user.LastName)
             .NotEmpty()
             .MinimumLength(3)
             .MaximumLength(10);

            RuleFor(user => user.Password)
             .NotEmpty()
             .MinimumLength(8);

            RuleFor(user => user.Email)
             .NotEmpty()
             .EmailAddress();
        }
    }
}
