using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SonoTracker.Common.Core;
using SonoTracker.Common.Helpers.HttpClient;
using SonoTracker.Common.Helpers.HttpClient.RestSharp;
using SonoTracker.Common.Helpers.MediaUploader;
using SonoTracker.Common.Helpers.TokenGenerator;
using SonoTracker.Common.Services;

namespace SonoTracker.Common.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ConfigureDependencyExtension
    {
        public static IServiceCollection RegisterCommonServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors();
            services.RegisterMainCore();
            services.RegisterHttpClientHelpers();
            return services;
        }

        private static void RegisterMainCore(this IServiceCollection services)
        {
            services.AddSingleton<MicroServicesUrls>();
            services.AddScoped<IClaimService, ClaimService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IResponseResult, ResponseResult>();
            services.AddTransient<IFinalResult, FinalResult>();
            services.AddTransient<ITokenGenerator, TokenGenerator>();
            services.AddTransient<IUploaderConfiguration, UploaderConfiguration>();
        }

        private static void RegisterHttpClientHelpers(this IServiceCollection services)
        {
            services.AddTransient<IRestSharpClient, RestSharpClient>();
        }

    }
}
