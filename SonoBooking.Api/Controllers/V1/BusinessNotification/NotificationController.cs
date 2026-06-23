using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SonoBooking.Api.Controllers.V1.Base;
using SonoBooking.Application.Services.BusinessNotification.Notification;
using SonoBooking.Common.Core;
using System.Threading;

namespace SonoBooking.Api.Controllers.V1.BusinessNotification
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/notifications")]
    [Authorize]
    public class NotificationController(INotificationService notificationService) : BaseController
    {
        [HttpGet]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetNotificationsAsync(
            [FromQuery] int take = 20,
            [FromQuery] bool onlyUnread = false,
            CancellationToken cancellationToken = default)
        {
            IFinalResult res = await notificationService.GetNotificationsAsync(take, onlyUnread, cancellationToken);
            return StatusCode((int)res.Status, res);
        }

        [HttpGet("unread-count")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetUnreadCountAsync(
            CancellationToken cancellationToken = default)
        {
            IFinalResult res = await notificationService.GetUnreadCountAsync(cancellationToken);
            return StatusCode((int)res.Status, res);
        }

        [HttpPatch("{id}/read")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IFinalResult>> MarkAsReadAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            IFinalResult res = await notificationService.MarkAsReadAsync(id, cancellationToken);
            return StatusCode((int)res.Status, res);
        }

        [HttpPatch("read-all")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> MarkAllAsReadAsync(
            CancellationToken cancellationToken = default)
        {
            IFinalResult res = await notificationService.MarkAllAsReadAsync(cancellationToken);
            return StatusCode((int)res.Status, res);
        }
    }
}
