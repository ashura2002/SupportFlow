using Application.Features.Dashboards.Queries;
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
    [Authorize(Roles = TenantRoles.TenantAdmin)]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<DashboardResponse>> GetDashboard(CancellationToken ct)
        {
            var query = new TenantAdminDashboardQuery();
            var result = await _mediator.Send(query, ct);
            return this.ToActionResult(result);
        }

    }
}
