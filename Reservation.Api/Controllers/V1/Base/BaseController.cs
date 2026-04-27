using Microsoft.AspNetCore.Mvc;

namespace SonoTracker.Api.Controllers.V1.Base
{
    /// <inheritdoc />
    /// <remarks>
    /// Add <c>CancellationToken cancellationToken</c> to action parameters to support request cancellation
    /// (e.g. client disconnect). It is bound to <see cref="HttpContext.RequestAborted"/> by default.
    /// </remarks>
    [ApiController]
    public class BaseController : ControllerBase
    {
    }
}