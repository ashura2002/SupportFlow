using Application.Common.Models;
using Application.Common.Results;
using MediatR;

namespace Application.Features.Tickets.Commands
{
    public sealed record ReplyToTicketCommand(
        Guid TicketId,
        string Message,
        IReadOnlyCollection<UploadImage> Attachments) : IRequest<Result>;
}