using Application.Features.Tenants.Commands;
using Application.Features.Tenants.Queries;
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
    public class TenantsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TenantsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Guid>> RegisterTenant(
            [FromBody] RegisterCompanyRequest request,
            CancellationToken ct)
        {
            var command = new RegisterCompanyCommand(
                request.TenantName,
                request.Identifier,
                request.FirstName,
                request.LastName,
                request.Password,
                request.Email);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);
        }


        [Authorize(Roles = PlatformRoles.PlatformAdmin)]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<TenantResponse>>> GetAllTenants(
            [FromQuery] PaginatedRequest request,
            CancellationToken ct)
        {
            var query = new GetAllTenantsQuery(request.Page, request.PageSize);
            var result = await _mediator.Send(query, ct);
            return this.ToActionResult(result);

        }


        [Authorize(Roles = TenantRoles.TenantAdmin)]
        [HttpPut]
        public async Task<ActionResult> UpdateCompanyName(
            [FromBody] UpdateNameRequest request,
            CancellationToken ct)
        {
            var command = new UpdateNameCommand(request.CompanyName);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);

        }
    }
}