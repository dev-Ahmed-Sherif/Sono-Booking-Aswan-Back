using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SonoBooking.Api.Controllers.V1.Base;
using SonoBooking.Application.Services.BusinessNotification.Chat;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.BusinessNotification.Chat;
using System.Threading;

namespace SonoBooking.Api.Controllers.V1.BusinessNotification
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/chat")]
    [Authorize]
    public class ChatController(IChatService chatService) : BaseController
    {
        [HttpGet("conversations")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetConversationsAsync(CancellationToken cancellationToken = default)
        {
            IFinalResult res = await chatService.GetConversationsAsync(cancellationToken);
            return StatusCode((int)res.Status, res);
        }

        [HttpGet("contacts")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetChatContactsAsync(CancellationToken cancellationToken = default)
        {
            IFinalResult res = await chatService.GetChatContactsAsync(cancellationToken);
            return StatusCode((int)res.Status, res);
        }

        [HttpGet("online-status")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetOnlineStatusesAsync(
            [FromQuery] string[] userIds,
            CancellationToken cancellationToken = default)
        {
            IFinalResult res = await chatService.GetOnlineStatusesAsync(
                userIds ?? Array.Empty<string>(),
                cancellationToken);
            return StatusCode((int)res.Status, res);
        }

        [HttpGet("conversations/{id}/messages")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetMessagesAsync(
            string id,
            [FromQuery] int take = 50,
            [FromQuery] string? before = null,
            CancellationToken cancellationToken = default)
        {
            IFinalResult res = await chatService.GetMessagesAsync(id, take, before, cancellationToken);
            return StatusCode((int)res.Status, res);
        }

        [HttpPost("conversations")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status201Created)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IFinalResult>> CreateConversationAsync(
            [FromBody] CreateChatConversationDto dto,
            CancellationToken cancellationToken = default)
        {
            IFinalResult res = await chatService.CreateConversationAsync(dto.ParticipantUserIds, cancellationToken);
            return StatusCode((int)res.Status, res);
        }

        [HttpPost("messages")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IFinalResult>> SendMessageAsync(
            [FromBody] SendChatMessageDto dto,
            CancellationToken cancellationToken = default)
        {
            IFinalResult res = await chatService.SendMessageAsync(
                dto.ConversationId,
                dto.Content,
                publishRealtime: true,
                cancellationToken);

            return StatusCode((int)res.Status, res);
        }

        [HttpGet("requests/{requestId}/conversations")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IFinalResult>> GetRequestConversationsAsync(
            string requestId,
            [FromQuery] string? groupType = null,
            CancellationToken cancellationToken = default)
        {
            IFinalResult res = string.IsNullOrWhiteSpace(groupType)
                ? await chatService.GetRequestConversationsAsync(requestId, cancellationToken)
                : await chatService.GetOrCreateRequestConversationAsync(requestId, groupType, cancellationToken);

            return StatusCode((int)res.Status, res);
        }

        [HttpPost("requests/{requestId}/conversations")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status201Created)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IFinalResult>> CreateRequestConversationAsync(
            string requestId,
            [FromBody] GetRequestChatConversationDto dto,
            CancellationToken cancellationToken = default)
        {
            IFinalResult res = await chatService.GetOrCreateRequestConversationAsync(
                requestId,
                dto.GroupType,
                cancellationToken);

            return StatusCode((int)res.Status, res);
        }
    }
}
