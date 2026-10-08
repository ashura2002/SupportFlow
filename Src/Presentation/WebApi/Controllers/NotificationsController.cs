using Application.Features.Notifications.Commands;
using Application.Features.Notifications.Queries;
using Application.ResponseDTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;

namespace WebApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<NotificationResponse>>> GetAllMyNotifications(CancellationToken ct)
        {
            var query = new GetAllNotificationQuery();
            var result = await _mediator.Send(query, ct);
            return this.ToActionResult(result);
        }

        [HttpPut("{notificationId:guid}")]
        public async Task<ActionResult> MarkAsRead([FromRoute] Guid notificationId, CancellationToken ct)
        {
            var command = new MarkAsReadCommand(notificationId);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);
        }


        [HttpDelete("{notificationId:guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid notificationId, CancellationToken ct)
        {
            var command = new DeleteNotificationCommand(notificationId);
            var result = await _mediator.Send(command, ct);
            return this.ToActionResult(result);
        }
    }
}
