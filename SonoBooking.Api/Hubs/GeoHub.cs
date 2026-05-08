using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SonoBooking.Domain.Entities.Identity;
using SonoTracker.Application.Services.Tracker.FloatingUnits;
using SonoTracker.Application.Services.Tracker.TripGeo;
using SonoTracker.Common.DTO.Tracker.FloatingUnit;
using SonoTracker.Common.DTO.Tracker.TripGeo;
using StackExchange.Redis;
using System.Security.Claims;


namespace SonoBooking.Api.Hubs
{

    /// <summary>  
    /// Represents a SignalR hub for managing geolocation-related communication.  
    /// </summary> 
    //[Authorize]
    public class GeoHub(
        UserManager<User> userManager,
        ITripGeoService tripGeoService,
        IFloatingUnitService floatingUnitService) : Hub
    {
        /// <inheritdoc/>
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("FloatingUnitLocations", await LiveLocations());
        }
        private async Task<IEnumerable<EditTripGeoDto>> LiveLocations()
        {
            // First North , Second East 
            //var waterBordersNorthWest = "(25.207514, 32.675006) (25.227416, 32.634516) (25.219218, 32.652795)";
            //var waterBordersNorthEast = "25.230196, 32.635972  25.207386, 32.675426";

            // List to hold last locations of floating units in Aswan
            var lastLocations = new List<EditTripGeoDto>();
            // Define water borders for Aswan to Luxor
            var waterBordersArrayAswanLuxor = new[]
            {
                new
                {
                    id = "b1e1e1a0-1a1a-4a1a-9a1a-000000000001",
                    North =  25.207514,
                    East = 32.675006,
                    Name = "NorthWest1"
                },
                new
                {
                    id = "b1e1e1a0-1a1a-4a1a-9a1a-000000000002",
                    North = 25.227416,
                    East = 32.634516,
                    Name = "NorthWest2"
                }
            };
            // (V.I.P) Should be done Last location of floating unit should be equal to the last Tourist Marina Location
            // Ensure `floatingUnits` is not null before accessing its properties

            var floatingUnitsResult = await floatingUnitService.GetAllAsync();
            if (floatingUnitsResult.Data is ICollection<FloatingUnitDto> floatingUnits)
            {
                for (int i = 0; i < floatingUnits.ToList().Count; i++)
                {
                    // Your logic here
                    var floatingUnitLastResult = await tripGeoService.GetLastByFloatingUnitIdAsync(floatingUnits.ElementAt(i).Id);
                    if (floatingUnitLastResult.Data is EditTripGeoDto lastLocation)
                    {
                        if (lastLocation != null)
                        {
                            // Check if the last location is within the defined water borders
                            if (double.Parse(lastLocation.GeoPointNorth) <= waterBordersArrayAswanLuxor[0].North ||
                                (double.Parse(lastLocation.GeoPointEast) <= waterBordersArrayAswanLuxor[0].East ||
                                 double.Parse(lastLocation.GeoPointEast) <= waterBordersArrayAswanLuxor[1].East)
                                )
                            {
                                // The last location is within the water borders
                                lastLocations.Add(lastLocation);
                            }
                            else
                            {
                                // The last location is outside the water borders
                                continue; // Skip adding this location to the list
                            }
                        }
                    }
                }
            }

            // Ensure `tripGeoService.GetAllAsync()` result is not null before using it
            return lastLocations;
        }

        #region Redis Cashing Location Methods
        private async Task<EditTripGeoDto?> GetLocationFromRedis(string userId)
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            var db = redis.GetDatabase();
            var locationData = await db.StringGetAsync(userId);
            if (!locationData.IsNullOrEmpty)
            {
                return System.Text.Json.JsonSerializer.Deserialize<EditTripGeoDto>(locationData);
            }
            return null;
        }
        #endregion

        private async Task SendLocation(EditTripGeoDto location)
        {
            var senderId = await GetSenderIdAsync();
            if (senderId != null)
            {
                // Optionally, you can save the location to the database here using tripGeoService
                await SendLocationUpdate(senderId, location);
            }
        }
        private async Task SendLocationUpdate(string userId, EditTripGeoDto location)
        {
            await Clients.User(userId).SendAsync("ReceiveLocationUpdate", location);
        }
        private async Task<string?> GetSenderIdAsync()
        {
            return Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}

