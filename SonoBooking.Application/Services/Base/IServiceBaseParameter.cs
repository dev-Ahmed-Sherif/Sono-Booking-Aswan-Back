using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SonoTracker.Common.Core;
using SonoTracker.Common.Infrastructure.UnitOfWork;
using SonoTracker.Integration.CacheRepository;

namespace SonoTracker.Application.Services.Base
{
    public interface IServiceBaseParameter<T> where T : class
    {
        IMapper Mapper { get; set; }

        IUnitOfWork<T> UnitOfWork { get; set; }

        IResponseResult ResponseResult { get; set; }

        IHttpContextAccessor HttpContextAccessor { get; set; }

        IConfiguration Configuration { get; set; }

        ICacheRepository CacheRepository { get; set; }

        ILogger<T> Logger { get; set; }
    }
}