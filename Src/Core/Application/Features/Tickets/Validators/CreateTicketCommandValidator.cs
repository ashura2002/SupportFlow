using Application.Features.Tickets.Commands;
using FluentValidation;

namespace Application.Features.Tickets.Validators
{
    public sealed class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
    {
        public CreateTicketCommandValidator()
        {
            RuleFor(t => t.Title)
                .MinimumLength(3)
                .NotEmpty();

            RuleFor(t => t.Description)
                .NotEmpty();

            RuleFor(t => t.Priority)
                .NotEmpty();

            RuleFor(t => t.CategoryId)
                .NotEmpty();
        }
    }
}
