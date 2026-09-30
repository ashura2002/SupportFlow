using Application.Features.Categories.Commands;
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
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = Role.Administrator)]
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateCategory([FromBody] CreateCategoryRequest request, CancellationToken ct)
        {
            var command = new CreateCategoryCommand(request.Name, request.Description);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);
        }


    }
}
