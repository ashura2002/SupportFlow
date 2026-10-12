using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.TicketAudits.Queries;

public sealed class GetTicketAuditHistoryQueryHandler : IRequestHandler<GetTicketAuditHistoryQuery, Result<IReadOnlyList<TicketAuditResponse>>>
{
    private readonly ITicketAuditReadRepository _ticketAuditReadRepository;
    public GetTicketAuditHistoryQueryHandler(ITicketAuditReadRepository ticketAuditReadRepository)
    {
        _ticketAuditReadRepository = ticketAuditReadRepository;
    }

    public async Task<Result<IReadOnlyList<TicketAuditResponse>>> Handle(
        GetTicketAuditHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var ticketAudit = await _ticketAuditReadRepository.GetTicketAuditByIdAsync(request.TicketId, cancellationToken);
        return Result<IReadOnlyList<TicketAuditResponse>>.Success(ticketAudit);
    }
}
