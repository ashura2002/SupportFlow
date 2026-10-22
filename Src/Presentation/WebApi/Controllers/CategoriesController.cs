using Application.Features.Categories.Commands;
using Application.Features.Categories.Queries;
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
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = TenantRoles.TenantAdmin)]
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateCategory([FromBody] CreateCategoryRequest request, CancellationToken ct)
        {
            var command = new CreateCategoryCommand(request.Name, request.Description);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);
        }



        [Authorize(Roles = TenantRoles.TenantAdmin)]
        [HttpPatch("{categoryId:guid}")]
        public async Task<ActionResult> UpdateCategory(
            [FromRoute] Guid categoryId,
            [FromBody] UpdateCategoryRequest request,
            CancellationToken ct)
        {
            var command = new UpdateCategoryCommand(categoryId, request.Name, request.Description);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);
        }


        [Authorize(Roles = TenantRoles.TenantAdmin)]
        [HttpDelete("{categoryId:guid}")]
        public async Task<ActionResult> DeleteCategory(
            [FromRoute] Guid categoryId,
            CancellationToken ct)
        {
            var command = new DeleteCategoryCommand(categoryId);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);
        }


        [Authorize(Roles = $"{TenantRoles.Requester},{TenantRoles.SupportAgent},{TenantRoles.TenantAdmin}")]
        [HttpGet]
        [EnableRateLimiting("GetResourcesPolicy")]
        public async Task<ActionResult<PaginatedResult<CategoryResponse>>> GetAllCategories(
            [FromQuery] PaginatedRequest request,
            CancellationToken ct)
        {
            var command = new GetAllCategoriesQuery(request.Page, request.PageSize);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);
        }
    }
}
