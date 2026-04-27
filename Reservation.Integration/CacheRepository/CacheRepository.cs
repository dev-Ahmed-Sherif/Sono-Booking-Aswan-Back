using System.Threading.Tasks;
using SonoTracker.Common.Caching.Redis;
using SonoTracker.Common.Helpers.HttpClient.RestSharp;

namespace SonoTracker.Integration.CacheRepository
{
    public class CacheRepository : ICacheRepository
    {
        private readonly IRestSharpClient _restSharpClient;
        public CacheRepository(IRestSharpClient restSharpClient)
        {
            _restSharpClient = restSharpClient;
        }


        //public async Task<EmployeeProfileDto> GetEmployeeAsync(string nationalId)
        //{
        //    var employee =  RedisCacheHelper.GetT<EmployeeProfileDto>(nationalId);
        //    return await Task.FromResult(employee);
        //}
    }
}
