using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.TicketAudits.Queries;

public sealed class GetTicketAuditHistoryQueryHandler : IRequestHandler<GetTicketAuditHistoryQuery, Result<IReadOnlyList<TicketAuditResponse>>>
{
    private readonly ITicketAuditReadRepository _ticketAuditReadRepository;
    private readonly ICurrentUserService _currentUserService;
    public GetTicketAuditHistoryQueryHandler(
        ITicketAuditReadRepository ticketAuditReadRepository,
        ICurrentUserService currentUserService)
    {
        _ticketAuditReadRepository = ticketAuditReadRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<IReadOnlyList<TicketAuditResponse>>> Handle(
        GetTicketAuditHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var ticketAudit = await _ticketAuditReadRepository.GetTicketAuditByIdAsync(
            request.TicketId,
            _currentUserService.TenantId,
            cancellationToken);
        return Result<IReadOnlyList<TicketAuditResponse>>.Success(ticketAudit);
    }
}
