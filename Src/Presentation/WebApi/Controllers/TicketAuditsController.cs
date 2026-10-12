
using Application.Features.TicketAudits.Queries;
using Application.ResponseDTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Constant;
using WebApi.Extensions;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketAuditsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TicketAuditsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = Role.Administrator)]
        [HttpGet("{ticketId:guid}/details")]
        public async Task<ActionResult<IReadOnlyList<TicketAuditResponse>>> GetAuditByTicketId(
            [FromRoute] Guid ticketId,
            CancellationToken ct
        )
        {
            var query = new GetTicketAuditHistoryQuery(ticketId);
            var result = await _mediator.Send(query, ct);
            return this.ToActionResult(result);
        }

    }
}
