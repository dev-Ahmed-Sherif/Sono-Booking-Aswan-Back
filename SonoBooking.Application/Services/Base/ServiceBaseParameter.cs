using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SonoTracker.Common.Core;
using SonoTracker.Common.Infrastructure.UnitOfWork;
using SonoTracker.Integration.CacheRepository;

namespace SonoTracker.Application.Services.Base
{
    public class ServiceBaseParameter<T> : IServiceBaseParameter<T> where T : class
    {

        public ServiceBaseParameter(
            IMapper mapper,
            IUnitOfWork<T> unitOfWork,
            IResponseResult responseResult,
            IHttpContextAccessor httpContextAccessor,
            ICacheRepository cacheRepository,
            IConfiguration configuration,
            ILogger<T> logger
        )
        {
            Mapper = mapper;
            UnitOfWork = unitOfWork;
            ResponseResult = responseResult;
            HttpContextAccessor = httpContextAccessor;
            CacheRepository = cacheRepository;
            Configuration = configuration;
            Logger = logger;
        }

        public IMapper Mapper { get; set; }

        public IUnitOfWork<T> UnitOfWork { get; set; }

        public IResponseResult ResponseResult { get; set; }

        public IHttpContextAccessor HttpContextAccessor { get; set; }

        public ICacheRepository CacheRepository { get; set; }

        public IConfiguration Configuration { get; set; }

        public ILogger<T> Logger { get; set; }
    }
}