using Application.Features.Tickets.Commands;
using FluentValidation;

namespace Application.Features.Tickets.Validators
{
    public sealed class AssignAgentCommandValidator : AbstractValidator<AssignAgentCommand>
    {
        public AssignAgentCommandValidator()
        {
            RuleFor(x => x.SupportAgentId)
                .NotEmpty();

            RuleFor(x => x.TicketId)
                .NotEmpty();
        }
    }
}
