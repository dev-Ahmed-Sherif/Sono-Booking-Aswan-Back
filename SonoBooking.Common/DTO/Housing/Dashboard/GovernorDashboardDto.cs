using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.Dashboard;

[ExcludeFromCodeCoverage]
public class GovernorDashboardDto
{
    public int TodayTotalRequests { get; set; }
    public int TodayApprovedRequests { get; set; }
    public int TodayRejectedRequests { get; set; }
    public int OccupancyPercent { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<ApartmentOccupancyItemDto> ApartmentOccupancy { get; set; } = [];
    public List<ApprovedRequestItemDto> LatestApprovedRequests { get; set; } = [];
    public List<DashboardDailyStatDto> DailyStats { get; set; } = [];
}

[ExcludeFromCodeCoverage]
public class ApartmentOccupancyItemDto
{
    public string UnitLabel { get; set; } = string.Empty;
    public int Percent { get; set; }
}

[ExcludeFromCodeCoverage]
public class ApprovedRequestItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
}

[ExcludeFromCodeCoverage]
public class DashboardDailyStatDto
{
    public string Date { get; set; } = string.Empty;
    public int TotalRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int RejectedRequests { get; set; }
    public decimal TotalRevenue { get; set; }
}
