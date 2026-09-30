using Application.Features.Auth.Command;
using Application.ResponseDTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;
using WebApi.Request;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var command = new LoginCommand(
                request.Email,
                request.Password);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);
        }
    }
}
