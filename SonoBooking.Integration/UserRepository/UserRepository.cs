using SonoTracker.Common.Helpers.HttpClient;
using SonoTracker.Common.Helpers.HttpClient.RestSharp;

namespace SonoTracker.Integration.UserRepository
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
