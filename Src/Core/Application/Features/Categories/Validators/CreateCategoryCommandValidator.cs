
using Application.Features.Categories.Commands;
using FluentValidation;

namespace Application.Features.Categories.Validators
{
    public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(c => c.Name)
                .MinimumLength(3)
                .NotEmpty();
        }
    }
}
