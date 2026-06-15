using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SonoBooking.Api.Controllers.V1.Base;
using SonoBooking.Application.Services.Housing.Dashboard;
using SonoBooking.Common.Core;
using System.Threading;

namespace SonoBooking.Api.Controllers.V1.Housing;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class DashboardController(IDashboardService dashboardService) : BaseController
{
    [HttpGet("getGovernorSummary")]
    [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IFinalResult>> GetGovernorSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        IFinalResult res = await dashboardService.GetGovernorSummaryAsync(cancellationToken);
        return Ok(res);
    }
}
