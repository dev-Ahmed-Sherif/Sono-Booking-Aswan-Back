using System;

namespace SonoBooking.Common.Services
{
    public interface IClaimService
    {
        string UserId { get; }
        string Token { get; }
    }
}

