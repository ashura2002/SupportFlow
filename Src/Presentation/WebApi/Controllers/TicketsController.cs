using Application.Features.Tickets.Commands;
using Application.Features.Tickets.Queries;
using Application.ResponseDTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Constant;
using WebApi.Extensions;
using WebApi.Request;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TicketsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [Authorize(Roles = Role.Requester)]
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateTicket([FromBody] CreateTicketRequest request, CancellationToken ct)
        {
            var command = new CreateTicketCommand(request.Title, request.Description, request.Priority, request.CategoryId);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);
        }

        [Authorize(Roles = $"{Role.Administrator},{Role.SupportAgent}")]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<TicketResponse>>> GetAllTickets([FromQuery] PaginatedRequest request, CancellationToken ct)
        {
            var query = new GetAllTicketsQuery(request.Page, request.PageSize);
            var result = await _mediator.Send(query, ct);
            return this.ToActionResult(result);
        }

        [Authorize(Roles = $"{Role.Administrator},{Role.SupportAgent}")]
        [HttpGet("{ticketId:guid}/details")]
        public async Task<ActionResult<TicketDetailsResponse>> GetTicketById([FromRoute] Guid ticketId, CancellationToken ct)
        {
            var query = new GetTicketByIdQuery(ticketId);
            var result = await _mediator.Send(query, ct);
            return this.ToActionResult(result);
        }


        [Authorize(Roles = Role.Requester)]
        [HttpGet("my-tickets")]
        public async Task<ActionResult<PaginatedResult<TicketResponse>>> GetAllMyTickets([FromQuery] PaginatedRequest request, CancellationToken ct)
        {
            var query = new GetAllMyTicketsQuery(request.Page, request.PageSize);
            var result = await _mediator.Send(query, ct);
            return this.ToActionResult(result);
        }


        [Authorize(Roles = Role.Requester)]
        [HttpGet("my-tickets{ticketId:guid}/details")]
        public async Task<ActionResult<TicketDetailsResponse>> GetMyTicketById([FromRoute] Guid ticketId, CancellationToken ct)
        {
            var query = new GetMyTicketByIdQuery(ticketId);
            var result = await _mediator.Send(query, ct);
            return this.ToActionResult(result);
        }

        [Authorize(Roles = Role.Administrator)]
        [HttpPut("assign-agent/{ticketId:guid}")]
        public async Task<ActionResult> AssignAgent([FromRoute] Guid ticketId, [FromBody] AssignAgentRequest request, CancellationToken ct)
        {
            var command = new AssignAgentCommand(request.AgentId, ticketId);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);

        }

        [Authorize(Roles = Role.SupportAgent)]
        [HttpGet("my-assign-tickets")]
        public async Task<ActionResult<PaginatedResult<TicketResponse>>> GetAllMyAssignTickets(
            [FromQuery] PaginatedRequest request, CancellationToken ct)
        {
            var query = new GetAllMyAssignTicketsQuery(request.Page, request.PageSize);
            var result = await _mediator.Send(query, ct);
            return this.ToActionResult(result);
        }


        [Authorize(Roles = Role.SupportAgent)]
        [HttpPost("{ticketId:guid}/start")]
        public async Task<ActionResult> StartTicketProgress([FromRoute] Guid ticketId, CancellationToken ct)
        {
            var command = new StartTicketProgressCommand(ticketId);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);
        }

        [Authorize(Roles = Role.SupportAgent)]
        [HttpPost("{ticketId:guid}/wait-for-user")]
        public async Task<ActionResult> WaitForUser([FromRoute] Guid ticketId, CancellationToken ct)
        {
            var command = new WaitForUserCommand(ticketId);
            var result = await _mediator.Send(command, ct);

            return this.ToActionResult(result);
        }

        [Authorize(Roles = $"{Role.Requester},{Role.SupportAgent}")]
        [HttpPost("{ticketId:guid}/reply")]
        public async Task<ActionResult> ReplyToTicket([FromRoute] Guid ticketId, [FromBody] ReplyToTicketRequest request, CancellationToken ct)
        {
            var command = new ReplyToTicketCommand(ticketId, request.Message);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);
        }

        [Authorize(Roles = Role.SupportAgent)]
        [HttpPost("{ticketId:guid}/resolve")]
        public async Task<ActionResult> ResolveTicket([FromRoute] Guid ticketId, CancellationToken ct)
        {
            var command = new ResolveTicketCommand(ticketId);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);
        }

        [Authorize(Roles = Role.Requester)]
        [HttpPost("{ticketId:guid}/close")]
        public async Task<ActionResult> CloseTicket([FromRoute] Guid ticketId, CancellationToken ct)
        {
            var command = new CloseTicketCommand(ticketId);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);
        }

        [Authorize(Roles = Role.Requester)]
        [HttpPost("{ticketId:guid}/reopen")]
        public async Task<ActionResult> ReopenTicket([FromRoute] Guid ticketId, CancellationToken ct)
        {
            var command = new ReopenTicketCommand(ticketId);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);
        }

        [Authorize(Roles = Role.SupportAgent)]
        [HttpPost("{ticketId:guid}/start-progress-after-reopen")]
        public async Task<ActionResult> StartProgressAfterReopen([FromRoute] Guid ticketId, CancellationToken ct)
        {
            var command = new StartProgressAfterReopenCommand(ticketId);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);
        }

    }
}
