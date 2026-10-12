using Application.Common.Results;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.TicketAudits.Queries;

public sealed record GetTicketAuditHistoryQuery(Guid TicketId) : IRequest<Result<IReadOnlyList<TicketAuditResponse>>>;