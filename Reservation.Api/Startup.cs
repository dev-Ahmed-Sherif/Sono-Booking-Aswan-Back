using Asp.Versioning.ApiExplorer;
using SonoTracker.Api.Extensions;
using SonoTracker.Api.Hubs;
using SonoTracker.Api.MiddleWares;
using SonoTracker.Application.DependencyExtension;

namespace SonoTracker.Api
{
    /// <summary>
    /// Start Up Class
    /// </summary>
    public class Startup
    {
        private readonly Shell _shell;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            _shell = ((Shell)Activator.CreateInstance(typeof(Shell)))!;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        }
        /// <summary>
        /// Public Configuration Property
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configure Dependencies
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.RegisterServices(Configuration);

        }

        /// <summary>
        /// Configure Pipeline
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="provider"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env , IApiVersionDescriptionProvider provider)
        {
            _shell.ConfigureHttp(app, env);
            Shell.Start(_shell);
            app.Configure(Configuration , provider);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.ConfigureCustomMiddleware();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<GeoHub>("/hubs/geo");
                endpoints.MapControllers();
            });
        }
    }
}