using SonoBooking.Common.Helpers.HttpClient;
using SonoBooking.Common.Helpers.HttpClient.RestSharp;

namespace SonoBooking.Integration.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly IRestSharpClient _restSharpClient;
        private readonly MicroServicesUrls _urls;
        public UserRepository(IRestSharpClient restSharpClient, MicroServicesUrls urls)
        {
            _restSharpClient = restSharpClient;
            _urls = urls;
        }
    }
}

