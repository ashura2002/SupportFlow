using Application.Features.Tickets.Commands;
using FluentValidation;

namespace Application.Features.Tickets.Validators
{
    public sealed class ReplyToTicketRequestValidator:AbstractValidator<ReplyToTicketCommand>
    {
        public ReplyToTicketRequestValidator()
        {
            RuleFor(x => x.TicketId)
                .NotEmpty();

            RuleFor(x => x.Message)
                .MinimumLength(3)
                .NotEmpty();
        }
    }
}
