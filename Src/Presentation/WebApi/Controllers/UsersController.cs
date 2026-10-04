using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using Application.ResponseDTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApi.Constant;
using WebApi.Extensions;
using WebApi.Request;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = Role.Administrator)]
        [HttpPost("requester")]
        public async Task<ActionResult<Guid>> CreateRequester([FromBody] CreateRequesterRequest request, CancellationToken ct)
        {
            var command = new CreateRequesterCommand(
                request.FirstName,
                request.LastName,
                request.Password,
                request.Email);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);
        }


        [Authorize(Roles = Role.Administrator)]
        [HttpPost("supportAgent")]
        public async Task<ActionResult<Guid>> CreateSupportAgent([FromBody] CreateSupportAgentRequest request, CancellationToken ct)
        {
            var command = new CreateSupportAgentCommand(request.FirstName, request.LastName, request.Password, request.Email);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);
        }


        [HttpPut("password")]
        public async Task<ActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request, CancellationToken ct)
        {
            var command = new UpdatePasswordCommand(request.NewPassword, request.ConfirmNewPassword);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);
        }

   
        [HttpPatch]
        public async Task<ActionResult> UpdateDetails([FromBody] UpdateDetailsRequest request, CancellationToken ct)
        {
            var command = new UpdateDetailsCommand(request.FirstName, request.LastName);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);
        }


        [HttpGet("me")]
        [EnableRateLimiting("GetResourcesPolicy")]
        public async Task<ActionResult<UserResponse>> GetMe(CancellationToken ct)
        {
            var query = new GetMeQuery();
            var result = await _mediator.Send(query, ct);
            return this.ToActionResult(result);
        }

        [Authorize(Roles = Role.Administrator)]
        [HttpGet("active-users")]
        [EnableRateLimiting("GetResourcesPolicy")]
        public async Task<ActionResult<PaginatedResult<UserResponse>>> GetAllActiveUsers(
            [FromQuery] PaginatedRequest request, 
            CancellationToken ct)
        {
            var query = new GetAllActiveUsersQuery(request.Page, request.PageSize);
            var result = await _mediator.Send(query, ct);
            return this.ToActionResult(result);
        }

        [Authorize(Roles = Role.Administrator)]
        [HttpGet("{userId:guid}/details")]
        [EnableRateLimiting("GetResourcesPolicy")]
        public async Task<ActionResult<UserResponse>> GetUserById([FromRoute] Guid userId, CancellationToken ct)
        {
            var query = new GetUserByIdQuery(userId);
            var result = await _mediator.Send(query, ct);
            return this.ToActionResult(result);
        }

        [HttpDelete("me")]
        public async Task<ActionResult> DeleteAccount(CancellationToken ct)
        {
            var command = new DeleteAccountCommand();
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);
        }
    }
}