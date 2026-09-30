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
        public async Task<ActionResult<TicketResponse>> GetTicketById([FromRoute] Guid ticketId, CancellationToken ct)
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
        public async Task<ActionResult<TicketResponse>> GetMyTicketById([FromRoute] Guid ticketId, CancellationToken ct)
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

        // my-assigned-tickets

        [Authorize(Roles = Role.SupportAgent)]
        [HttpPost("my-assigned-tickets/{ticketId:guid}/start")]
        public async Task<ActionResult> StartTicketProgress([FromRoute] Guid ticketId, CancellationToken ct)
        {
            var command = new StartTicketProgressCommand(ticketId);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);
        }


    }
}
